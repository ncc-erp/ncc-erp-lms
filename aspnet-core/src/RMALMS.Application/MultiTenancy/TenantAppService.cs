using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using RMALMS.Authorization;
using RMALMS.Authorization.Roles;
using RMALMS.Authorization.Users;
using RMALMS.Editions;
using RMALMS.MultiTenancy.Dto;
using RMALMS.IoC;
using RMALMS.Entities;
using RMALMS.Common;
using System;
using System.Collections.Generic;

namespace RMALMS.MultiTenancy
{
    [AbpAuthorize(PermissionNames.Pages_Tenants)]
    public class TenantAppService : AsyncCrudAppService<Tenant, TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>, ITenantAppService
    {
        private readonly TenantManager _tenantManager;
        private readonly EditionManager _editionManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IPermissionManager _permissionManager;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private IWorkScope _ws;
        

        public TenantAppService(
            IRepository<Tenant, int> repository, 
            TenantManager tenantManager, 
            EditionManager editionManager,
            UserManager userManager,            
            RoleManager roleManager, 
            IAbpZeroDbMigrator abpZeroDbMigrator, 
            IPasswordHasher<User> passwordHasher, 
            IPermissionManager permissionManager,
            IWorkScope ws
            ) 
            : base(repository)
        {
            _ws = ws;
            _tenantManager = tenantManager; 
            _editionManager = editionManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            _passwordHasher = passwordHasher;
            _permissionManager = permissionManager;
        }
        
        public override async Task<TenantDto> Create(CreateTenantDto input)
        {
            
            CheckCreatePermission();

            // Create tenant
            var tenant = ObjectMapper.Map<Tenant>(input);
            tenant.ConnectionString = input.ConnectionString.IsNullOrEmpty()
                ? null
                : SimpleStringCipher.Instance.Encrypt(input.ConnectionString);

            var defaultEdition = await _editionManager.FindByNameAsync(EditionManager.DefaultEditionName);
            if (defaultEdition != null)
            {
                tenant.EditionId = defaultEdition.Id;
            }

            await _tenantManager.CreateAsync(tenant);
            await CurrentUnitOfWork.SaveChangesAsync(); // To get new tenant's id.

            // Create tenant database
            _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

            // We are working entities of new tenant, so changing tenant filter
            using (CurrentUnitOfWork.SetTenantId(tenant.Id))
            {
                // Create static roles for new tenant: System Administrator Role
                CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));

                await CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

                // Grant all permissions to admin role
                // assign permissions to system admin role
                var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                var allPermissions = _permissionManager.GetAllPermissions(tenancyFilter: false).ToList();
                await AssignPermissionToRole(adminRole, allPermissions);
                //await _roleManager.GrantAllPermissionsAsync(adminRole);

                // Create admin user for the tenant
                var adminUser = User.CreateTenantAdminUser(tenant.Id, input.AdminEmailAddress);
                adminUser.CreatorUserId = AbpSession.UserId;
                await _userManager.InitializeOptionsAsync(tenant.Id);
                CheckErrors(await _userManager.CreateAsync(adminUser, input.DefaultPassword));
                await CurrentUnitOfWork.SaveChangesAsync(); // To get admin user's id

                // Assign admin user to role!
                CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));
                //await CurrentUnitOfWork.SaveChangesAsync();

                /* Course Administrator Role*/
                var couseAdminName = StaticRoleNames.Tenants.CourseAdmin;
                var courseAdminRole = new Role
                {
                    Name = couseAdminName,
                    DisplayName = StaticRoleNames.Tenants.CourseAdmin,
                    Description = StaticRoleNames.Tenants.CourseAdmin,
                    IsStatic = false
                };
                courseAdminRole.SetNormalizedName();
                await _roleManager.CreateAsync(courseAdminRole);//wait for creating courseAdminRole completed
                await CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

                //// assign course admin to permission
                //var allPermissions = _permissionManager.GetAllPermissions(tenancyFilter: false).ToList();
                //await AssignPermissionToRole(courseAdminRole, allPermissions);

                  /* Student Role */
                  var studentRole = new Role
                {
                    Name = StaticRoleNames.Tenants.Student,
                    DisplayName = StaticRoleNames.Tenants.Student,                    
                    Description = StaticRoleNames.Tenants.Student,
                    IsStatic = false
                };
                studentRole.SetNormalizedName();
                await _roleManager.CreateAsync(studentRole);//wait for creating studentRole completed
                await CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

                // assign permissions to course admin role                
                await AssignPermissionToRole(courseAdminRole, allPermissions);

                // assign permissions to student role
                await AssignPermissionToRole(studentRole, allPermissions);                               

                //insert 3 static UserStatus to db
                await _ws.GetRepo<Entities.UserStatus>().InsertAsync(new Entities.UserStatus
                {
                    Level = (int)UserStatusLevel.Basic,
                    DisplayName = "Basic",
                    IsStatic = true,
                    TenantId = tenant.Id
                });
                await _ws.GetRepo<Entities.UserStatus>().InsertAsync(new Entities.UserStatus
                {
                    Level = (int)UserStatusLevel.Proficient,
                    DisplayName = "Proficient",
                    IsStatic = true,
                    TenantId = tenant.Id
                });
                await _ws.GetRepo<Entities.UserStatus>().InsertAsync(new Entities.UserStatus
                {
                    Level = (int)UserStatusLevel.Advanced,
                    DisplayName = "Advanced",
                    IsStatic = true,
                    TenantId = tenant.Id
                });

                //insert 3 static CourseStatus to db
                await _ws.GetRepo<CourseLevel>().InsertAsync(new CourseLevel
                {
                    Level = (int)CourseStatusLevel.Basic,
                    DisplayName = "Basic",
                    IsStatic = true,
                    TenantId = tenant.Id
                });
                await _ws.GetRepo<CourseLevel>().InsertAsync(new CourseLevel
                {
                    Level = (int)CourseStatusLevel.Advance,
                    DisplayName = "Advance",
                    IsStatic = true,
                    TenantId = tenant.Id
                });
                await _ws.GetRepo<CourseLevel>().InsertAsync(new CourseLevel
                {
                    Level = (int)CourseStatusLevel.Expert,
                    DisplayName = "Expert",
                    IsStatic = true,
                    TenantId = tenant.Id
                });

                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return MapToEntityDto(tenant);
        }

        private async Task AssignPermissionToRole(Role r, List<Permission> listPermissions)
        {
            List<string> permissions = GrantPermissionRoles.PermissionRoles.ContainsKey(r.Name) ? GrantPermissionRoles.PermissionRoles[r.Name] : new List<string>();
            if (permissions != null && permissions.Count > 0)
            {
                var dbpermissions = listPermissions.Where(s => permissions.Contains(s.Name));
                await _roleManager.SetGrantedPermissionsAsync(r, dbpermissions);
            }
        }

        protected override void MapToEntity(TenantDto updateInput, Tenant entity)
        {
            // Manually mapped since TenantDto contains non-editable properties too.
            entity.Name = updateInput.Name;
            entity.TenancyName = updateInput.TenancyName;
            entity.IsActive = updateInput.IsActive;
        }

        public override async Task Delete(EntityDto<int> input)
        {
            CheckDeletePermission();

            var tenant = await _tenantManager.GetByIdAsync(input.Id);
            await _tenantManager.DeleteAsync(tenant);
        }

        private void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
