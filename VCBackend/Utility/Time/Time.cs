using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Utility.Time
{
    public class Time
    {
        private static DateTime ZERO_TIME = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static TimeSpan ValidityInHours(double hours)
        {
            return TimeSpan.FromTicks(((DateTime.UtcNow.AddHours(hours)) - ZERO_TIME).Ticks);
        }
    }
}