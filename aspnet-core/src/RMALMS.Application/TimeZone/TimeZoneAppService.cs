using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RMALMS.DomainServices;
using RMALMS.Entities;
using RMALMS.TimeZone.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RMALMS.TimeZone
{
    public class TimeZoneAppService : ApplicationBaseService
    {
        public async Task<List<UserTimeZoneDto>> GetAll()
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                return await _ws.GetAll<UserTimeZone>().ProjectTo<UserTimeZoneDto>().ToListAsync();
            }
        }
    }
}
