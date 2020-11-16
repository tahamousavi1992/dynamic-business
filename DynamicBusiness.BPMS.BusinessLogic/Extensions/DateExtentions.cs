using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public static class DateExtentions
    {
        /// <summary>
        /// used in event because I set week from 1 to 7 and datetime week is from 1 to 0.
        /// </summary>
        public static int GetDayOfWeekBy7(this DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return 1;
                case DayOfWeek.Tuesday:
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                case DayOfWeek.Sunday:
                    return 7;
                default:
                    return 0;
            }
        }


        public static bool AreInTheSameWeek(this DateTime date1, DateTime date2)
        {
            var d1 = date1.Date.AddDays(-1 * (int)date1.DayOfWeek);
            var d2 = date2.Date.AddDays(-1 * (int)date2.DayOfWeek);
            return d1 == d2;
        }

        public static bool AreInTheSameMonth(this DateTime date1, DateTime date2)
        {
            var d1 = date1.Date.AddDays(-1 * date1.Day);
            var d2 = date2.Date.AddDays(-1 * date2.Day);
            return d1 == d2;
        }

    }
}
