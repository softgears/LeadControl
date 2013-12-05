using System;

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
    }
}