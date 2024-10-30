using Abp.Application.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services.Dto;
using RMALMS.Groups.Dto;
using RMALMS.Roles.Dto;
using System.Threading.Tasks;
using Abp.Authorization;
using RMALMS.Entities;
using System;
using RMALMS.Authorization.Roles;
using Abp.Domain.Repositories;
using RMALMS.Authorization.Users;
using RMALMS.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Abp.IdentityFramework;
using System.Collections.Generic;
using RMALMS.IoC;
using AutoMapper.QueryableExtensions;
using Abp.UI;

namespace RMALMS.Groups
{
    
    [AbpAuthorize]
    public class GroupAppService : AsyncCrudAppService<Group, GroupDto, Guid, PagedResultRequestDto, CreateGroupDto, GroupDto>, IGroupAppService
    {
       
        private readonly IWorkScope _workScope;
        public GroupAppService(
            IRepository<Group, Guid> repository,
            IWorkScope workScope
         )
            : base(repository)
        {
            _workScope = workScope;
        }
        public ListResultDto<PermissionDto> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();
            return new ListResultDto<PermissionDto>();            
        }

        public async Task<ListResultDto<GroupDto>> GetAllGroups()
        {
            var query = Repository.GetAllIncluding(g => g.Parent).ProjectTo<GroupDto>();
            var groups = await query.ToListAsync();
            return new ListResultDto<GroupDto>(groups);            
        }

        public override async Task<GroupDto> Create(CreateGroupDto input)
        {
            CheckCreatePermission();
            var isExisting = await _workScope.GetAll<Group>().AnyAsync(g => g.Name == input.Name);
            if (isExisting) throw new UserFriendlyException(String.Format("Duplicate group name {0}", input.Name));
            var group = ObjectMapper.Map<Group>(input);

            group.Id = await _workScope.InsertAndGetIdAsync(group);

            return MapToEntityDto(group);
        }

        public override async Task<GroupDto> Update(GroupDto input)
        {      
            CheckUpdatePermission();
            var isExisting = await _workScope.GetAll<Group>().AnyAsync(g => g.Name == input.Name && g.Id != input.Id);
            if (isExisting) throw new UserFriendlyException(String.Format("Duplicate group name {0}", input.Name));

            var group = await _workScope.GetRepo<Group, Guid>().FirstOrDefaultAsync(g => g.Id == input.Id);
            MapToEntity(input, group);
            await _workScope.GetRepo<Group, Guid>().UpdateAsync(group);

            return input;
            
        }

    }
}
