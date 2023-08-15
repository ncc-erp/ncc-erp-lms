using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RMALMS.DomainServices;
using RMALMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RMALMS.Localization;
using RMALMS.Languages.Dto;

namespace RMALMS.Languages
{
    public class LanguagesAppService : ApplicationBaseService
    {
        public async Task<List<LanguagesDto>> GetAll()
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                return await _ws.GetAll<Abp.Localization.ApplicationLanguage,int>().ProjectTo<LanguagesDto>().ToListAsync();
            }
        }
    }
}
