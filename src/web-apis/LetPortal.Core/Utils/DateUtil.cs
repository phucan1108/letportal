using System;
using TimeZoneConverter;

namespace LetPortal.Core.Utils
{
    public class DateUtil
    {
        public static DateTime GetCurrentSystemDateByTz(string timezone)
        {
            // Due to TimeZone problem per OS, we need to use another lib to exchange corresponding TZ
            var tz = TZConvert.GetTimeZoneInfo(timezone);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        }

        public static DateTime GetDateByTz(DateTime convert, string timezone)
        {
            // Due to TimeZone problem per OS, we need to use another lib to exchange corresponding TZ
            var tz = TZConvert.GetTimeZoneInfo(timezone);
            return TimeZoneInfo.ConvertTimeFromUtc(convert, tz);
        }

        /// <summary>
        ///     Format: YYYYMM
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetShortMonthOfDate(DateTime dateTime)
        {
            return dateTime.Year + dateTime.Month.ToString();
        }
    }
}