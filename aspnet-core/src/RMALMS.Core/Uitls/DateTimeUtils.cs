using Abp.Timing;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMALMS.Uitls
{
    public class DateTimeUtils
    {
        // All now function use Clock.Provider.Now
        public static DateTime GetNow()
        {
            return Clock.Provider.Now;
        }
    }
}
