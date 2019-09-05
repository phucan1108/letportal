using System;

namespace LetPortal.Core.Utils
{
    public class DateUtil
    {
        public static DateTime GetCurrentSystemDateByTz(string timezone)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        }
        
        public static DateTime GetDateByTz(DateTime convert, string timezone)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
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