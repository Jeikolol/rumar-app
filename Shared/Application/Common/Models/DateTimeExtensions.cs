using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Application.Common.Models
{
    public static class DateTimeExtensions
    {
        public static DateTime UtcToLocal(this DateTime source, string localTimeZoneId = "SA Western Standard Time")
        {
            if (string.IsNullOrWhiteSpace(localTimeZoneId))
                localTimeZoneId = "SA Western Standard Time";

            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(localTimeZoneId);

            return TimeZoneInfo.ConvertTimeFromUtc(source, localTimeZone);
        }

        public static DateTime UtcToLocal(this DateTime source, TimeZoneInfo localTimeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(source, localTimeZone);
        }
    }

}
