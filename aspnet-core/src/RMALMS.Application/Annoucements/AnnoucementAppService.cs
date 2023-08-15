using Abp.Authorization;
using RMALMS.Entities;
using RMALMS.Roles.Dto;
using RMALMS.Annoucements.Dto;
using System;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using RMALMS.Paging;
using RMALMS.IoC;
using Abp.Domain.Repositories;
using System.Linq;
using AutoMapper.QueryableExtensions;
using RMALMS.Extension;
using RMALMS.Authorization.Users;

namespace RMALMS.Annoucements
{
    [AbpAuthorize]
    public class AnnoucementAppService : CrudApplicationBaseService<Annoucement, AnnoucementDto, Guid, PagedResultRequestDto, AnnoucementDto, AnnoucementDto>
    {
        private readonly IWorkScope _ws;
        public AnnoucementAppService(IRepository<Annoucement, Guid> respository, IWorkScope ws)
            : base(respository)
        {
            _ws = ws;
        }

        public ListResultDto<PermissionDto> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();
            return new ListResultDto<PermissionDto>();
        }

        [HttpPost]
        public async Task<GridResult<AnnoucementDto>> getAnnoucementByCourseInstanceIdPagging(GridAnnoucementParam input)
        {

            var queryresult = from a in Repository.GetAll()
                              where a.CourseInstanceId == input.courseInstanceId
                              join u in WorkScope.GetAll<User, long>() on a.CreatorUserId equals u.Id
                              select new AnnoucementDto
                              {
                                  Id = a.Id,
                                  Title = a.Title,
                                  Content = a.Content,
                                  CourseInstanceId = a.CourseInstanceId,
                                  ImageCover = u.Avatar != null  ? u.Avatar : "assets/images/user.png",
                                  UserName = u.FullName,
                                  CreationTime = a.CreationTime
                              };
            return await queryresult.GetGridResult(queryresult, input.input);
        }

        public async override Task<AnnoucementDto> Create(AnnoucementDto input)
        {
            var item = ObjectMapper.Map<Annoucement>(input);
            item.Id = Guid.Empty;
            item.Id = await _ws.InsertAndGetIdAsync(item);
            return ObjectMapper.Map<AnnoucementDto>(item);
        }
        public async override Task<AnnoucementDto> Update(AnnoucementDto input)
        {
            var item = await Repository.GetAsync(input.Id);
            ObjectMapper.Map(input, item);
            await Repository.UpdateAsync(item);
            return input;
        }

        #region for student

       [HttpPost]
        public async Task<GridResult<AnnoucementStudentDto>> GetAnnoucementForStudentByCourseInstanceIdPagging(GridAnnoucementParam input)
        {
            var qAnnoucement = (from annoucement in _ws.GetRepo<Annoucement, Guid>().GetAll().Where(ci => ci.CourseInstanceId == input.courseInstanceId)
                                join user in _ws.GetRepo<User, long>().GetAll()
                                on annoucement.CreatorUserId equals user.Id
                                select new AnnoucementStudentDto
                                {
                                    Id = annoucement.Id,
                                    Content = annoucement.Content,
                                    Title = annoucement.Title,
                                    CreationTime = annoucement.CreationTime,
                                    ImageCover = user.Avatar,
                                    UserName = user.UserName,
                                    FullName = user.FullName,
                                    Email = user.EmailAddress
                                });

       
            return await qAnnoucement.OrderByDescending(m => m.CreationTime).GetGridResult(qAnnoucement, input.input);
        }
        #endregion

    }
}
