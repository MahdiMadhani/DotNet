using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InMemoryCaching.Models
{
    public class FormattingService
    {
        public string AsLongReadableDate(DateTime? dt)
        {
            if (dt == null)
                return "فاقد تاریخ";
            FarsiLibrary.Utils.PersianDate pd = (DateTime)dt;
            return string.Format("{0} {1} {2} {3}", GetPersianDayOfWeek(pd.DayOfWeek), GetPersianNumber(pd.Day),
                pd.LocalizedMonthName, GetPersianNumber(pd.Year));
        }

        private string GetPersianNumber(int day)
        {
            return day.ToString();
        }

        private string GetPersianDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Saturday:
                    return "شنبه";
                case DayOfWeek.Sunday:
                    return "یک شنبه";
                case DayOfWeek.Monday:
                    return "دو شنبه";
                case DayOfWeek.Tuesday:
                    return "سه شنبه";
                case DayOfWeek.Wednesday:
                    return "چهار شنبه";
                case DayOfWeek.Thursday:
                    return "پنج شنبه";
                case DayOfWeek.Friday:
                    return "جمعه";
            }
            return "";
        }
        
        
        
        
    }
}
