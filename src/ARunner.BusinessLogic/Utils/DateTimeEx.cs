using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ARunner.BusinessLogic.Utils
{
    public static class DateTimeEx
    {
        public static int GetWeekOfYear(this DateTime date)
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date.AddDays(4 - (day == 0 ? 7 : day)), 
                                                                     CalendarWeekRule.FirstFourDayWeek, 
                                                                     DayOfWeek.Monday);
        }

        public static DateTime FirstDayOfWeek(this DateTime date, DayOfWeek dayOfWeek = DayOfWeek.Monday)
        {
            var delta = dayOfWeek - date.DayOfWeek;
            if (delta > 0)
                delta -= 7;
            return date.AddDays(delta);
        }

        public static DateTime LastDayOfWeek(this DateTime date, DayOfWeek dayOfWeek = DayOfWeek.Monday)
        {
            return FirstDayOfWeek(date, dayOfWeek).AddDays(6);
        }
    }
}
