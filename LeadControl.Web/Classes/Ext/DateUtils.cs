using System;

namespace LeadControl.Web.Classes.Ext
{
    /// <summary>
    /// Статический класс по работе с датами
    /// </summary>
    public static class DateUtils
    {
        public static string[] Names = new[]
                            {
                                "январь", "февраль", "март", "апрель", "май", "июнь", "июль", "август", "сентябрь",
                                "октябрь", "ноябрь", "декабрь"
                            };
        
        /// <summary>
        /// Возвращает название месяца по его номера
        /// </summary>
        /// <param name="number">Номер месяца</param>
        /// <returns></returns>
        public static string GetMonthName(int number)
        {
            return Names[number - 1];
        }

        /// <summary>
        /// Возвращает номер последнего дня в году
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        public static int LastDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1).Day;
        }

        /// <summary>
        /// Возвращает первый день в году
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        public static DateTime FirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static int GetDayNumberOnAWeek(DayOfWeek dayOfWeek)
        {
            switch(dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return 7;
                    break;
                case DayOfWeek.Monday:
                    return 1;
                    break;
                case DayOfWeek.Tuesday:
                    return 2;
                    break;
                case DayOfWeek.Wednesday:
                    return 3;
                    break;
                case DayOfWeek.Thursday:
                    return 4;
                    break;
                case DayOfWeek.Friday:
                    return 5;
                    break;
                case DayOfWeek.Saturday:
                    return 6;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("dayOfWeek");
            }
        }

        /// <summary>
        /// Возвращает дату начала недели для указанной даты
        /// </summary>
        /// <param name="dt">Дата и время</param>
        /// <returns></returns>
        public static DateTime WeekStart(this DateTime dt)
        {
            if (dt.DayOfWeek == DayOfWeek.Monday)
            {
                return dt.Date;
            }
            else
            {
                return dt.AddDays(-1*(DateUtils.GetDayNumberOnAWeek(dt.DayOfWeek)-1));
            }
        }
    }
}