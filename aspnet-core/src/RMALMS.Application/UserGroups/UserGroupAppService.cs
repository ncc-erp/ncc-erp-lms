using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using RMALMS.Entities;
using RMALMS.IoC;
using RMALMS.Roles.Dto;
using RMALMS.UserGroups.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using RMALMS.Authorization.Users;
using RMALMS.Extension;
using RMALMS.Paging;
using RMALMS.Authorization.Roles;
using Abp.Authorization.Users;
using Microsoft.AspNetCore.Mvc;

namespace RMALMS.UserGroups
{
    //[AbpAuthorize(Authorization.PermissionNames.Pages_UserGroups)]
    [AbpAuthorize]
    public class UserGroupAppService : AsyncCrudAppService<UserGroup, UserGroupDto, Guid, PagedResultRequestDto, CreateUserGroupDto, UserGroupDto>, IUserGroupAppService
    {
        private readonly IWorkScope _ws;


        public UserGroupAppService(IRepository<UserGroup, Guid> repository, IWorkScope workScope) : base(repository)
        {
            this._ws = workScope;
        }


        public async Task AddUsersToGroupAsync(UsersToGroupDto input)
        {

            //Check group existing
            var isExistGroup = await _ws.GetRepo<Group>().GetAll().AnyAsync(g => g.Id == input.GroupId);

            if (!isExistGroup) throw new UserFriendlyException("The group is not exist");

            var alreadyList = await _ws.GetRepo<UserGroup>().GetAll().Where(ug => ug.GroupId == input.GroupId).Select(ug => ug.UserId).ToListAsync();

            //insert 
            var insertList = input.UserIds.Except(alreadyList);
            foreach (var userId in insertList)
            {
                var userGroup = new UserGroup
                {
                    UserId = userId,
                    GroupId = input.GroupId
                };
                await _ws.InsertAsync(userGroup);
            }

            //delete
            var deleteList = alreadyList.Except(input.UserIds);
            foreach (var userId in deleteList)
            {
                await _ws.GetRepo<UserGroup>().DeleteAsync(ug => ug.UserId == userId && ug.GroupId == input.GroupId);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public ListResultDto<PermissionDto> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();
            return new ListResultDto<PermissionDto>();
        }

        public override Task<UserGroupDto> Update(UserGroupDto input)
        {
            throw new NotImplementedException();
        }

        public override async Task<UserGroupDto> Create(CreateUserGroupDto input)
        {
            var isExist = await _ws.GetRepo<UserGroup>().GetAll().AnyAsync(ug => ug.GroupId == input.GroupId && ug.UserId == input.UserId);

            if (isExist) throw new UserFriendlyException("The Student is already belong to this group");

            var isGroupExist = await _ws.GetRepo<Entities.Group>().GetAll().AnyAsync(g => g.Id == input.GroupId);

            if (!isGroupExist) throw new UserFriendlyException(string.Format("The group id {0} is not exist", input.GroupId));

            var isUserExist = await _ws.GetRepo<User, long>().GetAll().AnyAsync(u => u.Id == input.UserId);

            if (!isUserExist) throw new UserFriendlyException(string.Format("The user id {0} is not exist", input.UserId));

            var userGroup = ObjectMapper.Map<UserGroup>(input);
            userGroup.Id = await _ws.InsertAndGetIdAsync(userGroup);

            return MapToEntityDto(userGroup);

        }


        public async Task DeleteMulti(DeleteMultiUserGroupDto input)
        {
            await _ws.GetRepo<UserGroup>().DeleteAsync(ug => input.Ids.Contains(ug.Id));
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<GridResult<UserGroupDto>> GetAllAsync(GridParam input)
        {
            // lay all bao gom ca User va Group
            var query = Repository.GetAllIncluding(s => s.User, s => s.Group).Select(s => new UserGroupDto
            {
                GroupId = s.GroupId,
                GroupName = s.Group.Name,
                UserId = s.UserId,
                Id = s.Id,
                UserName = s.User.Name
            });
            return await query.GetGridResult(query, input);
        }

        public async Task<ListResultDto<UsersByGroupDto>> getUsersByGroupId(Guid groupId)
        {
            var query = Repository.GetAllIncluding(ug => ug.User).Where(ug => ug.GroupId == groupId).Select(u => new UsersByGroupDto
            {
                Id = u.UserId,
                Name = u.User.UserName
            });
            var result = await query.ToListAsync();
            return new ListResultDto<UsersByGroupDto>(result);
        }

        public async Task<ListResultDto<GroupsByUsrerDto>> getGroupsByUserId(long userId)
        {
            var query = Repository.GetAllIncluding(ug => ug.Group).Where(ug => ug.UserId == userId).Select(u => new GroupsByUsrerDto { Id = u.GroupId, Name = u.Group.Name });
            var result = await query.ToListAsync();
            return new ListResultDto<GroupsByUsrerDto>(result);
        }

        /// <summary>
        /// Save groups to 1 user
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AddGroupsToUserAsync(GroupsToUserDto input)
        {

            //Check user existing
            var isExistUser = await _ws.GetRepo<User, long>().GetAll().AnyAsync(u => u.Id == input.UserId);

            if (!isExistUser) throw new UserFriendlyException(string.Format("The user id {0} is not exist", input.UserId));

            var alreadyList = await _ws.GetRepo<UserGroup>().GetAll().Where(ug => ug.UserId == input.UserId).Select(ug => ug.GroupId).ToListAsync();

            //insert
            var insertList = input.GroupIds.Except(alreadyList);
            foreach (var groupId in insertList)
            {
                var userGroup = new UserGroup
                {
                    UserId = input.UserId,
                    GroupId = groupId
                };
                await _ws.InsertAsync<UserGroup>(userGroup);
            }

            //delete
            var deleteList = alreadyList.Except(input.GroupIds);
            await _ws.GetRepo<UserGroup>().DeleteAsync(ug => deleteList.Contains(ug.GroupId) && ug.UserId == input.UserId);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<string> CreateGroup(GroupCreateDto input)
        {
            CheckCreatePermission();
            var isExisting = await _ws.GetAll<Group>().AnyAsync(g => g.Name == input.Name.Trim());
            if (isExisting) throw new UserFriendlyException(String.Format("Group name {0} existed!", input.Name));

            var item = new Group
            {
                Name = input.Name.Trim(),
                Description = input.Description,
            };
            item.Id = await _ws.InsertAndGetIdAsync(item);
            return item.Id.ToString();
        }

        public async Task<bool> UpdateGroup(GroupUpdateDto input)
        {
            CheckUpdatePermission();
            var isExisting = await _ws.GetAll<Group>().AnyAsync(g => g.Name == input.Name.Trim() && g.Id != input.Id);
            if (isExisting) // throw new UserFriendlyException(String.Format("Duplicate group name {0}", input.Name));
            {
                throw new UserFriendlyException(String.Format("Group name {0} existed!", input.Name));
            }
            var item = await _ws.GetRepo<Group>().GetAsync(input.Id);
            item.Name = input.Name.Trim();
            item.Description = input.Description;
            await _ws.GetRepo<Group>().UpdateAsync(item);
            return true;
        }

        public async Task<ListResultDto<GroupIncludeUserDto>> GetsGroupIncludeUser()
        {
            var qUserGr = (from usergroup in _ws.GetRepo<UserGroup, Guid>().GetAll()
                          join user in _ws.GetRepo<User, long>().GetAll()
                          on usergroup.UserId equals user.Id into users
                          from us in users.DefaultIfEmpty()
                          select new UserGroupDto
                          {
                              Id = usergroup.Id,
                              UserId = usergroup.UserId,
                              GroupId = usergroup.GroupId,
                              UserName = us.UserName,
                              FullName = us.FullName,
                              ImageCover = us.Avatar,
                              Email = us.EmailAddress,
                          }).OrderBy(m => m.FullName);

            var test1 = qUserGr.ToList();
            List<UserGroup> test2 = _ws.GetRepo<UserGroup, Guid>().GetAll().ToList();

            var qUserGroup = from gr in _ws.GetRepo<Group, Guid>().GetAll()
                             join ug in qUserGr
                             on gr.Id equals ug.GroupId into grs
                             select new GroupIncludeUserDto
                             {
                                 Id = gr.Id,
                                 Name = gr.Name,
                                 Description = gr.Description,
                                 UserGroups = grs, 
                                 CreationTime = gr.CreationTime,
                             };

            var result = await qUserGroup.OrderByDescending(m => m.CreationTime).ToListAsync();
            return new ListResultDto<GroupIncludeUserDto>(result);

        }

        public async Task<ListResultDto<UserStudentDto>> GetsUserAsStudent()
        {
            var qRole = from role in _ws.GetRepo<Role, int>().GetAll().Where(m => m.Name == StaticRoleNames.Tenants.Student)
                        join userRole in _ws.GetRepo<UserRole, long>().GetAll()
                        on role.Id equals userRole.RoleId
                        select new
                        {
                            UserId = userRole.UserId
                        };          

            var qUser = from user in _ws.GetRepo<User, long>().GetAll().Where(m => m.IsActive == true)
                        join role in qRole
                        on user.Id equals role.UserId

                        join ugroup in _ws.GetRepo<UserGroup, Guid>().GetAll()
                        on user.Id equals ugroup.UserId into ugroups
                        select new UserStudentDto
                        {
                            UserId = user.Id,
                            Email = user.EmailAddress,
                            FullName = user.FullName,
                            ImageCover = user.Avatar,
                            UserName = user.UserName,
                            CountGroup = ugroups.Count(),
                        };

            var result = await qUser.OrderBy(m => m.UserName).ToListAsync();
            return new ListResultDto<UserStudentDto>(result);
        }

        public async Task<string> CreateUserGroup(UserGroupInput input)
        {
            CheckUpdatePermission();
            var isExisting = await _ws.GetAll<UserGroup>().AnyAsync(g => g.UserId == input.UserId && g.GroupId == input.GroupId);
            if (isExisting) 
            {
                throw new UserFriendlyException(String.Format("Existed student in group!"));
            }
            var item = new UserGroup
            {
                GroupId = input.GroupId,
                UserId = input.UserId
            };
            item.Id = await _ws.InsertAndGetIdAsync(item);
            return item.Id.ToString();
        }

        public async Task<bool> UpdateUserGroup(UserGroupInput input)
        {
            CheckUpdatePermission();
            var item = await _ws.GetAll<UserGroup>().FirstOrDefaultAsync(g => g.UserId == input.UserId && g.GroupId == input.GroupId_old);
            if (item != null)
            {
                item.GroupId = input.GroupId;
                await _ws.GetRepo<UserGroup>().UpdateAsync(item);
                return true;
            }
            return false;
        }

        [HttpPost]
        public async Task<bool> DeleteUserGroup(UserGroupInput input)
        {
            CheckUpdatePermission();
            var item = await _ws.GetAll<UserGroup>().FirstOrDefaultAsync(g => g.UserId == input.UserId && g.GroupId == input.GroupId);
            if(item == null)
            {
                throw new UserFriendlyException(String.Format("Data not found"));
            }
            await _ws.GetRepo<UserGroup, Guid>().DeleteAsync(item);
            return true;
        }

        public async Task<bool> DeleteGroup(Guid id)
        {
            CheckUpdatePermission();
            await _ws.GetRepo<Group, Guid>().DeleteAsync(id);
            return true;
        }


    }
}
