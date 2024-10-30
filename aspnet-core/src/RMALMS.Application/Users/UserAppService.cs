using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.IdentityFramework;
using Abp.Localization;
using Abp.Runtime.Session;
using RMALMS.Authorization;
using RMALMS.Authorization.Roles;
using RMALMS.Authorization.Users;
using RMALMS.Roles.Dto;
using RMALMS.Users.Dto;
using Abp.UI;
using Microsoft.Extensions.Options;
using RMALMS.Paging;
using RMALMS.Extension;
using Abp.Authorization.Users;
using Microsoft.AspNetCore.Mvc;
using RMALMS.Entities;
using RMALMS.Courses.Dto;
using RMALMS.DomainServices;
using System;
using RMALMS.IoC;
using System.Collections.ObjectModel;

namespace RMALMS.Users
{

    [AbpAuthorize(new[] { PermissionNames.Pages_Users, PermissionNames.Pages_UserGroups, PermissionNames.Pages_Account })]
    public class UserAppService : CrudApplicationBaseService<User, UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ICourseManager _courseManager;
        private readonly IUserServices _userService;
        private readonly IWorkScope _ws;

        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<Role> roleRepository,
            IUserServices userService,
            ICourseManager courseManager,
            IWorkScope workScope,
        IPasswordHasher<User> passwordHasher)
            : base(repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _courseManager = courseManager;
            _userService = userService;
            this._ws = workScope;
        }

        public override async Task<UserDto> Create(CreateUserDto input)
        {
            CheckCreatePermission();

            var user = ObjectMapper.Map<User>(input);
            user.CreatorUserId = AbpSession.UserId;

            user.TenantId = input.TenantId.HasValue ? input.TenantId : AbpSession.TenantId;
            user.IsEmailConfirmed = true;
            Entities.UserStatus userLevel = null;
            if (input.TenantId > 0)
            {
                using (CurrentUnitOfWork.SetTenantId(input.TenantId))
                {
                    if (input.RoleNames != null)
                    {
                        userLevel = await _ws.GetRepo<Entities.UserStatus, Guid>().GetAll().OrderBy(m => m.Level).FirstOrDefaultAsync();
                        //Assign roles
                        user.Roles = new Collection<UserRole>();
                        foreach (var roleName in input.RoleNames)
                        {
                            var role = await _roleManager.GetRoleByNameAsync(roleName);
                            user.Roles.Add(new UserRole(input.TenantId, user.Id, role.Id));
                        }
                    }
                }
            }
            else
            {
                if (input.RoleNames != null)
                {
                    userLevel = await _ws.GetRepo<Entities.UserStatus, Guid>().GetAll().OrderBy(m => m.Level).FirstOrDefaultAsync();
                    //Assign roles
                    user.Roles = new Collection<UserRole>();
                    foreach (var roleName in input.RoleNames)
                    {
                        var role = await _roleManager.GetRoleByNameAsync(roleName);
                        user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, role.Id));
                    }
                }
            }

            if (userLevel != null)
            {
                user.StatusId = userLevel.Id;
            }

            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

            CheckErrors(await _userManager.CreateAsync(user, input.Password));

            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(user);
        }


        public override async Task<UserDto> Update(UserDto input)
        {
            CheckUpdatePermission();

            var user = await _userManager.GetUserByIdAsync(input.Id);

            MapToEntity(input, user);

            CheckErrors(await _userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            return await Get(input);
        }


        public override async Task Delete(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            await _userManager.DeleteAsync(user);
        }

        [HttpPost]
        public async Task DeleteAdminWithTenantId(Id_TenantIdDto input)
        {
            if (input.TenantId > 0)
            {
                using (CurrentUnitOfWork.SetTenantId(input.TenantId))
                {
                    var user = await _userManager.GetUserByIdAsync(input.Id);
                    if (user == null) throw new UserFriendlyException("User does not exist");
                    await _userManager.DeleteAsync(user);
                }
            }
            else
            {
                var user = await _userManager.GetUserByIdAsync(input.Id);
                if (user == null) throw new UserFriendlyException("User does not exist");
                await _userManager.DeleteAsync(user);
            }

        }

        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }

        [HttpPost]
        public async Task<ListResultDto<RoleDto>> GetRolesByTenantId(Id_TenantIdDto input)
        {
            if (input.TenantId > 0)
            {
                using (CurrentUnitOfWork.SetTenantId(input.TenantId))
                {
                    var roles = await _roleRepository.GetAllListAsync();
                    return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
                }
            }
            else
            {
                var roles = await _roleRepository.GetAllListAsync();
                return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
            }

        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        protected override void MapToEntity(UserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

        protected override UserDto MapToEntityDto(User user)
        {
            var roles = _roleManager.Roles.Where(r => user.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.NormalizedName);
            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();
            return userDto;
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Roles);
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(User), id);
            }

            return user;
        }

        [HttpPost]
        public async Task<UserDto> GetAdminWithTenantId(Id_TenantIdDto input)
        {
            if (input.TenantId > 0)
            {
                using (CurrentUnitOfWork.SetTenantId(input.TenantId))
                {
                    var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == input.Id);
                    //var user = await _userManager.GetUserByIdAsync(input.Id);
                    if (user == null) throw new UserFriendlyException("User does not exist");
                    var userRole = base.MapToEntityDto(user);
                    userRole.RoleNames = _roleManager.Roles.Where(r => user.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.NormalizedName).ToArray();
                    return userRole;
                }
            }
            else
            {
                var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == input.Id);
                if (user == null) throw new UserFriendlyException("User does not exist");
                return MapToEntityDto(user);
            }
        }


        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedResultRequestDto input)
        {
            return query.OrderBy(r => r.UserName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto input)
        {
            if (input.TenantId <= 0)
            {
                var user = await _userManager.GetUserByIdAsync(input.Id);
                if (user == null) throw new UserFriendlyException("User does not exist");
                user.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(user, input.NewPassword);
                await _userManager.UpdateAsync(user);
            }
            else
            {
                using (CurrentUnitOfWork.SetTenantId(input.TenantId))
                {
                    var user = await _userManager.GetUserByIdAsync(input.Id);
                    if (user == null) throw new UserFriendlyException("User does not exist");
                    user.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(user, input.NewPassword);
                    await _userManager.UpdateAsync(user);
                }
            }

        }
        [AbpAuthorize(Authorization.PermissionNames.Pages_Account)]
        public async Task<bool> ChangePasswordAsync(ResetPasswordDto input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            var checkPW = await _userManager.CheckPasswordAsync(user, input.OldPassword);
            if (checkPW)
            {
                user.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(user, input.NewPassword);
                await _userManager.UpdateAsync(user);
                //var result = await _userManager.ChangePasswordAsync(user, input.NewPassword);
                return true;
            }
            return false;
        }


        public async Task<UserDto> GetUserById()
        {
            var userId = AbpSession.UserId;
            var query = await Repository.GetAll().FirstOrDefaultAsync(u => u.Id == userId);
            var user = ObjectMapper.Map<UserDto>(query);
            return user;
        }

        public async Task<GridResult<UserDto>> GetUsersByTenantIdAsync(UsersByTenantIdDto input)
        {
            int? tenant = AbpSession.TenantId;
            if (input.TenantId > 0) tenant = input.TenantId;

            //IQueryable<User> userNullTenant;          



            using (CurrentUnitOfWork.SetTenantId(tenant))
            {
                var currentUser = Repository.GetAll().Where( s=> s.TenantId == tenant && !s.IsDeleted);
                //var allUser = currentUser;

                var qusers = from user in currentUser
                             join userCreate in _ws.GetAll<User, long>().IgnoreQueryFilters().Where(s => !s.IsDeleted)
                             on user.CreatorUserId equals userCreate.Id into userCreates    
                             from creator in userCreates.DefaultIfEmpty()
                             select new UserDto
                             {
                                 Name = user.Name,
                                 UserName = user.UserName,
                                 Surname = user.Surname,
                                 EmailAddress = user.EmailAddress,
                                 IsActive = user.IsActive,
                                 FullName = user.Name + " " + user.Surname,
                                 Id = user.Id,                                 
                                 CreatorName = String.IsNullOrEmpty(creator.DisplayName) ? creator.UserName : creator.DisplayName,
                                 IsCreatedByRoot = !creator.TenantId.HasValue                                 
                             };

                var result = await qusers.GetGridResult(qusers, input);
                return result;
            }

        }



        protected override void CheckCreatePermission()
        {
            base.CheckCreatePermission();
        }

        public async Task<GridResult<UserBrifDto>> GetAllBrif(GridParam input)
        {
            var query = Repository.GetAllIncluding(u => u.Status).Select(u => new UserBrifDto
            {
                Id = u.Id,
                IsActive = u.IsActive,
                UserName = u.UserName,
                StatusName = u.Status.DisplayName
            });
            return await query.GetGridResult(query, input);
        }

        public async Task<GridResult<SelectableStudentDto>> GetStudents(GridParam input)
        {
            var quser =
                from u in _userService.GetUserByRole(StaticRoleNames.Tenants.Student)
                select new SelectableStudentDto
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    FirstName = u.Name,
                    LastName = u.Surname
                };

            return await quser.GetGridResult(quser, input);
        }


        public async Task<GridResult<SelectableStudentDto>> GetCourseAdmins(GridParam input)
        {
            var quser =
                from u in _userService.GetUserByRole(StaticRoleNames.Tenants.CourseAdmin)
                select new SelectableStudentDto
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    FirstName = u.Name,
                    LastName = u.Surname
                };

            return await quser.GetGridResult(quser, input);
        }


    }
}
