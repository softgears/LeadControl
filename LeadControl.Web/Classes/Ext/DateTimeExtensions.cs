using System;
using System.Collections.Generic;

namespace LeadControl.Web.Classes.Ext
{
    /// <summary>
    /// Статический класс помогающий форматировать даты
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Унифицированно преобразовывает дату в строку
        /// </summary>
        /// <param name="datetime">Дата</param>
        /// <returns>Строковое представление или пустая строка</returns>
        public static string FormatDate(this DateTime? datetime)
        {
            if (datetime == null || !datetime.HasValue)
            {
                return String.Empty;
            }
            return datetime.Value.FormatDate();
        }

        /// <summary>
        /// Унифицированно преобразует дату и время в строку
        /// </summary>
        /// <param name="datetime">Дата</param>
        /// <returns>Строковое представление</returns>
        public static string FormatDate(this DateTime datetime)
        {
            return datetime.ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// Унифицированно форматирует дату и время в строку
        /// </summary>
        /// <param name="datetime">Дата и время</param>
        /// <returns>Строковое представление даты и время</returns>
        public static string FormatDateTime(this DateTime? datetime)
        {
            if (datetime == null || !datetime.HasValue)
            {
                return String.Empty;
            }
            return datetime.Value.FormatDateTime();
        }

        /// <summary>
        /// Унифицированно преобразует дату и время в строку
        /// </summary>
        /// <param name="datetime">Дата и время</param>
        /// <returns>Дата и время в форме строки</returns>
        public static string FormatDateTime(this DateTime datetime)
        {
            return datetime.ToString("dd.MM.yyyy HH:mm:ss");
        }

        /// <summary>
        /// Возвращает читаемый вид даты и времени
        /// </summary>
        /// <param name="date">Дата и время</param>
        /// <returns></returns>
        public static string TimeAgo(this DateTime date)
        {
            if (date == DateTime.Now) return "только что";

            double difference = Math.Abs((DateTime.Now - date).TotalSeconds);

            List<string[]> periods = new List<string[]>();
            periods.Add(new string[] { "секунду", "секурнды", "секунд" });
            periods.Add(new string[] { "минуту", "минуты", "минут" });
            periods.Add(new string[] { "час", "часа", "часов" });
            periods.Add(new string[] { "день", "дня", "дней" });
            periods.Add(new string[] { "неделю", "недели", "недель" });
            periods.Add(new string[] { "месяц", "месяца", "месяцев" });
            periods.Add(new string[] { "год", "года", "лет" });
            periods.Add(new string[] { "десятилетие", "десятилетий", "десятилетий" });

            double[] lengths = new double[] { 60, 60, 24, 7, 4.35, 12, 10 };
            int[] cases = new int[] { 2, 0, 1, 1, 1, 2 };

            int j = 0;
            for (j = 0; difference >= lengths[j]; j++)
            {
                difference = difference / lengths[j];
            }

            difference = Math.Round(difference);

            string text = periods[j][(difference % 100 > 4 && difference % 100 < 20) ? 2 : cases[(int)Math.Min(difference % 10, 5)]];

            string prefix = date > DateTime.Now ? "через " : string.Empty;
            string suffix = date < DateTime.Now ? " назад" : string.Empty;

            return string.Format("{0}{1} {2}{3}", prefix, difference, text, suffix);
        }

        /// <summary>
        /// Возвращает читаемый вид даты и времени
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string TimeAgo(this DateTime? date)
        {
            return date.HasValue ? date.Value.TimeAgo() : String.Empty;
        }
    }
}