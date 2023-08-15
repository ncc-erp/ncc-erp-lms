using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using RMALMS.Authorization.Users;
using RMALMS.GradeSchemes.Dto;
using RMALMS.Entities;
using RMALMS.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.UI;
using AutoMapper.QueryableExtensions;

namespace RMALMS.GradeSchemes
{
    public class GradeSchemeElementAppService : CrudApplicationBaseService<GradeSchemeElement, GradeSchemeElementDto, Guid, PagedResultRequestDto, GradeSchemeElementDto, GradeSchemeElementDto>
    {
        private readonly IWorkScope _ws;
        public GradeSchemeElementAppService(IRepository<GradeSchemeElement, Guid> respository, IWorkScope ws)
            : base(respository)
        {
            _ws = ws;
        }

        public async override Task<GradeSchemeElementDto> Create(GradeSchemeElementDto input)
        {
            var item = ObjectMapper.Map<GradeSchemeElement>(input);
            item.Id = Guid.Empty;
            item.Id = await _ws.InsertAndGetIdAsync(item);
            return ObjectMapper.Map<GradeSchemeElementDto>(item);
        }

        public async override Task<GradeSchemeElementDto> Update(GradeSchemeElementDto input)
        {
            var item = await Repository.GetAsync(input.Id);
            ObjectMapper.Map(input, item);
            await Repository.UpdateAsync(item);
            return input;
        }
        public async Task<List<GradeSchemeElementDto>> GetElementsByGradeId(Guid gradeId)
        {
            var query = Repository.GetAll().Where(g => g.GradeSchemeId == gradeId).ProjectTo<GradeSchemeElementDto>();
            return await query.ToListAsync();
        }
    }
}
