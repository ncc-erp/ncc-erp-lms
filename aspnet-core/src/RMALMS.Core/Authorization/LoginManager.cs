using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.UI;
using Abp.Zero.Configuration;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using RMALMS.Authentication.Tokens;
using RMALMS.Authorization.Dto;
using RMALMS.Authorization.Roles;
using RMALMS.Authorization.Users;
using RMALMS.Configuration;
using RMALMS.Core.Authorization.Users;
using RMALMS.MultiTenancy;

namespace RMALMS.Authorization
{
    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {
        private readonly IConfiguration _configration;
        private readonly ILogger _logger;
        public LogInManager(
            UserManager userManager,
            IMultiTenancyConfig multiTenancyConfig,
            IRepository<Tenant> tenantRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IConfiguration configuration,
            ILogger<LogInManager> logger,
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
            IUserManagementConfig userManagementConfig,
            IIocResolver iocResolver,
            IPasswordHasher<User> passwordHasher,
            RoleManager roleManager,
            UserClaimsPrincipalFactory claimsPrincipalFactory)
            : base(
                  userManager,
                  multiTenancyConfig,
                  tenantRepository,
                  unitOfWorkManager,
                  settingManager,
                  userLoginAttemptRepository,
                  userManagementConfig,
                  iocResolver,
                  passwordHasher,
                  roleManager,
                  claimsPrincipalFactory)
        {
            _configration = configuration;
            _logger = logger;
        }
        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncNoPass(string token, string tenancyName = null, bool shouldLockout = true)
        {
            var result = await LoginAsyncInternalNoPass(token, tenancyName, shouldLockout);
            var user = result.User;
            await SaveLoginAttempt(result, tenancyName, user == null ? null : user.EmailAddress);
            return result;
        }

        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant, User>> LoginHashMezonAsnyc(MezonHashAuthDto hashAuthDto)
        {
            var result = await AuthMezonHashAsync(hashAuthDto);
            var user = result.User;
            await SaveLoginAttempt(result, hashAuthDto.TenancyName, user == null ? null : user.EmailAddress);
            return result;
        }
        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant, User>> LoginMezonAsnyc(string authCode, string redirectUri, string tenancyName = null)
        {
            var result = await AuthMezonServerAsync(authCode, redirectUri, tenancyName);
            var user = result.User;
            await SaveLoginAttempt(result, tenancyName, user == null ? null : user.EmailAddress);
            return result;
        }

        private async Task<AbpLoginResult<Tenant, User>> AuthMezonHashAsync(MezonHashAuthDto hashAuthDto)
        {
            if (hashAuthDto.HashData.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(hashAuthDto.HashData));
            }
            try
            {
                var appToken = _configration["Authentication:Mezon:AppToken"] ?? throw new ArgumentNullException("Invalid AppToken");
                var rawHashData = hashAuthDto.HashData.DecodeBase64();

                var hashData = HashParamsParser(rawHashData);
                var hashParams = new BaseHashData { query_id = hashData.query_id, user = hashData.user, auth_date = hashData.auth_date, signature = hashData.signature };
                var mezonUser = JsonConvert.DeserializeObject<MezonUser>(hashParams.user);

                byte[] secretKey = Hasher.HMAC_SHA256(Encoding.UTF8.GetBytes(appToken), Encoding.UTF8.GetBytes("WebAppData"));
                var hashedData = Hasher.HEX(Hasher.HMAC_SHA256(secretKey, Encoding.UTF8.GetBytes(HashParamsStringify(hashParams))));
            
                if (hashData.hash.Equals(hashedData) == false)
                    throw new UserFriendlyException("Authenticattion failed - Invalid hash key");

                _logger.LogWarning($"Try to login with user email: {mezonUser.mezon_id}");
                
                var loginResult = await HandleAuthWithEmail(mezonUser.mezon_id, hashAuthDto.TenancyName);
                return loginResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Authenticattion failed - Can't authenticate with Mezon server");
                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.UnknownExternalLogin, null);
            }
        }
        private async Task<AbpLoginResult<Tenant, User>> AuthMezonServerAsync(string authCode, string redirectUri, string tenancyName = null)
        {
            if (authCode.IsNullOrEmpty() || redirectUri.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(authCode));
            }
            try
            {
                var clientId = _configration["Authentication:Mezon:ClientId"] ?? throw new ArgumentNullException("Invalid ClientId");
                var clientSecret = _configration["Authentication:Mezon:ClientSecret"] ?? throw new ArgumentNullException("Invalid ClientSecret");
                var authServerUrl = _configration["Authentication:Mezon:AuthServerUrl"] ?? throw new ArgumentNullException("Invalid AuthServer");
                _logger.LogWarning($"Authenticating with Mezon server: {authServerUrl}, clientId: {clientId}, redirectUri: {redirectUri}");
                var restClient = new RestClient(authServerUrl);
                // Validate auth code
                var authRequest = new RestRequest("/oauth2/token", Method.POST);
                authRequest.AlwaysMultipartFormData = true;
                authRequest.AddHeader("Content-Type", "multipart/form-data");

                authRequest.AddParameter("grant_type", "authorization_code");
                authRequest.AddParameter("client_id", clientId);
                authRequest.AddParameter("code", authCode);
                authRequest.AddParameter("client_secret", clientSecret);
                authRequest.AddParameter("redirect_uri", redirectUri);

                var response = await restClient.ExecuteTaskAsync(authRequest);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new UserFriendlyException("Authenticattion failed - Can't verify auth code with Mezon server");
                }
                var authData = JsonConvert.DeserializeObject<MezonTokenData>(response.Content);
                if (authData is null || authData.access_token.IsNullOrEmpty())
                {
                    throw new UserFriendlyException("Authenticattion failed - Can't get access token from Mezon server");
                }

                // Get user info
                var userInfoRequest = new RestRequest("/userinfo", Method.GET);
                userInfoRequest.AddHeader("Authorization", $"Bearer {authData.access_token}");
                var userResponse = await restClient.ExecuteTaskAsync(userInfoRequest);
                if (userResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new UserFriendlyException("Authenticattion failed - Can't get user info from Mezon server");
                }

                var userData = JsonConvert.DeserializeObject<MezonUser>(userResponse.Content);
                _logger.LogWarning($"Try to login with user email: {userData.sub}");

                var loginResult = await HandleAuthWithEmail(userData.sub, tenancyName);
                return loginResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Authenticattion failed - Can't authenticate with Mezon server");
                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.UnknownExternalLogin, null);
            }
        }

        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncInternalNoPass(string token, string tenancyName, bool shouldLockout)
        {

            if (token.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(token));
            }
            
            try
            {
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token);
                var emailAddress = payload.Email;

                // checking
                var clientAppId = await SettingManager.GetSettingValueAsync(AppSettingNames.ClientAppId); //get clientAppId from setting
                var correctAudience = payload.AudienceAsList.Any(s => s == clientAppId);
                var correctIssuer = payload.Issuer == "accounts.google.com" || payload.Issuer == "https://accounts.google.com";
                var correctExpriryTime = payload.ExpirationTimeSeconds != null || payload.ExpirationTimeSeconds > 0;

                if (correctAudience && correctIssuer && correctExpriryTime)
                {
                    var loginResult = await HandleAuthWithEmail(emailAddress, tenancyName, shouldLockout);    
                    return loginResult;
                }
                else
                {
                    return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, null);
                }
            }
            catch (InvalidJwtException e)
            {
                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, null);
            }
        }

        private async Task<AbpLoginResult<Tenant, User>> HandleAuthWithEmail(string emailAddress, string tenancyName, bool shouldLockout = true)
        {
            Tenant tenant = null;

            //Get and check tenant
            using (UnitOfWorkManager.Current.SetTenantId(null))
            {
                if (!MultiTenancyConfig.IsEnabled)
                {
                    tenant = await GetDefaultTenantAsync();
                }

                else if (!string.IsNullOrWhiteSpace(tenancyName))
                {
                    tenant = await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenancyName);
                    if (tenant == null)
                    {
                        return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidTenancyName);
                    }

                    if (!tenant.IsActive)
                    {
                        return new AbpLoginResult<Tenant, User>(AbpLoginResultType.TenantIsNotActive, tenant);
                    }
                }
                var tenantId = tenant == null ? (int?)null : tenant.Id;
                using (UnitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await UserManager.InitializeOptionsAsync(tenantId);
                    var user = await UserManager.FindByNameOrEmailAsync(tenantId, emailAddress);
                    if (user == null)
                        throw new UserFriendlyException(string.Format("Login Fail - Account does not exist"));

                    var isLockOut = await UserManager.IsLockedOutAsync(user);
                    if (isLockOut)
                        return new AbpLoginResult<Tenant, User>(AbpLoginResultType.LockedOut, tenant, user);
                    var logỉnResult = await CreateLoginResultAsync(user, tenant);
                    return logỉnResult;
                }
            }
        }

        private HashData HashParamsParser(string queryString) {
            var queryParams = HttpUtility.ParseQueryString(queryString);
            var hashData = new HashData
            {
                query_id = queryParams["query_id"],
                user = queryParams["user"],
                auth_date = long.Parse(queryParams["auth_date"]),
                signature = queryParams["signature"],
                hash = queryParams["hash"]
            };
            return hashData;
        }
        private string HashParamsStringify(object hashData) {
            var queryString = new StringBuilder();

            var properties = hashData.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var value = property.GetValue(hashData);
                if (value != null)
                {
                    if (queryString.Length > 0)
                        queryString.Append("&");
                    queryString.AppendFormat($"{Uri.EscapeDataString(property.Name)}={Uri.EscapeDataString(value.ToString())}");
                }
            }

            return queryString.ToString();
        }
    }
}

