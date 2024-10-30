using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RMALMS.Authorization.Users;
using RMALMS.Entities;
using RMALMS.IoC;
using RMALMS.Roles.Dto;
using RMALMS.UserStatus.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.UserStatus
{
    //[AbpAuthorize(Authorization.PermissionNames.Pages_UserStatus)]
    [AbpAuthorize]
    public class UserStatusAppService : AsyncCrudAppService<Entities.UserStatus, UserStatusDto, Guid, PagedResultRequestDto, CreateUserStatusDto, UserStatusDto>, IUserStatusAppService
    {
        private readonly IWorkScope _ws;
        public UserStatusAppService(IRepository<Entities.UserStatus, Guid> repository, IWorkScope workScope) : base(repository)
        {
            this._ws = workScope;            
        }

        public ListResultDto<PermissionDto> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();
            return new ListResultDto<PermissionDto>();
        }

        public async Task<ListResultDto<UserStatusDto>> GetAllNotPagging()
        {
            var userStatus = await Repository.GetAll().OrderBy(m => m.Level).ToListAsync();
            return new ListResultDto<UserStatusDto>(ObjectMapper.Map<List<UserStatusDto>>(userStatus));
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_Settings)]
        public override async Task<UserStatusDto> Update(UserStatusDto input)
        {
            var exists = await _ws.GetAll<Entities.UserStatus>().AnyAsync(s => s.Id != input.Id && (s.Level == input.Level || s.DisplayName == input.DisplayName));
            if (exists)
            {
                throw new UserFriendlyException($"Level \"{input.Level}\"  or Name \"{ input.DisplayName }\" existed");
            }
            var userStatus = _ws.GetAll<Entities.UserStatus, Guid>().Where(us => us.Id == input.Id).FirstOrDefault();
            MapToEntity(input, userStatus);
            await _ws.GetRepo<Entities.UserStatus, Guid>().UpdateAsync(userStatus);
            return input;
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_Settings)]
        public override async Task<UserStatusDto> Create(CreateUserStatusDto input)
        {
            var exists = await _ws.GetAll<Entities.UserStatus>().AnyAsync(s => s.Level == input.Level || s.DisplayName == input.DisplayName);
            if (exists)
            {
                throw new UserFriendlyException($"Level \"{input.Level}\"  or Name \"{ input.DisplayName }\" existed");
            }
            var userStatus = ObjectMapper.Map<Entities.UserStatus>(input);
            await _ws.InsertAsync(userStatus);
            return MapToEntityDto(userStatus);
        }

        [AbpAuthorize(Authorization.PermissionNames.Pages_Settings)]
        public override async Task Delete(EntityDto<Guid> input)
        {
            await _ws.GetRepo<Entities.UserStatus>().DeleteAsync(us => us.Id == input.Id);
        }
        [AbpAuthorize(Authorization.PermissionNames.Pages_Settings)]
        public  async Task<int> CheckDelete(EntityDto<Guid> input)
        {
           return await _ws.GetAll<User, long>().Where(m => m.StatusId == input.Id).CountAsync();
        }
    }
}
