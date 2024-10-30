using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMALMS.EntityFrameworkCore.Seed.Host
{
    public class TimeZoneCreator
    {
        private readonly RMALMSDbContext _context;

        public TimeZoneCreator(RMALMSDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateUserTimeZone();
        }

        private void CreateUserTimeZone()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            var dbtimeZones = _context.UserTimeZones.IgnoreQueryFilters().Where(s => s.TenantId == null).ToList();


            foreach (var timezone in timeZones)
            {

                if (!dbtimeZones.Any(s => s.TimeZoneId == timezone.Id))
                {
                    _context.UserTimeZones.Add(new Entities.UserTimeZone
                    {
                        TimeZoneId = timezone.Id,
                        BaseUtcOffset = timezone.BaseUtcOffset.ToString(),
                        TenantId = null,
                        DisplayName = timezone.DisplayName,
                        StandardName = timezone.StandardName,
                        SupportsDaylightSavingTime = timezone.SupportsDaylightSavingTime
                    });
                }
            }
            _context.SaveChanges();
        }


    }
}
