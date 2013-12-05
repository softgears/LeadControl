// ============================================================
// 
// 	Asgard
// 	Asgard.Web.Public 
// 	MathUtils.cs
// 
// 	Created by: ykorshev 
// 	 at 03.08.2013 11:08
// 
// ============================================================

using System;

namespace LeadControl.Web.Classes.Utils
{
    /// <summary>
    /// Математические утилиты
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Возвращает процентную строку
        /// </summary>
        /// <param name="value">Малое</param>
        /// <param name="total">Большее</param>
        /// <returns></returns>
        public static string GetPercentage(int value, int total)
        {
            double v = value, t = total;
            var res = v/t*100;
            return string.Format("{0:0}%", res);
        }

        /// <summary>
        /// Возвращает процентную строку
        /// </summary>
        /// <param name="value">Малое</param>
        /// <param name="total">Большее</param>
        /// <returns></returns>
        public static string GetPercentage(long value, long total)
        {
            double v = value, t = total;
            var res = v / t * 100;
            return string.Format("{0:0}%", res);
        }

        /// <summary>
        /// Определяет количество страниц получающееся при разбиении набора по странично
        /// </summary>
        /// <param name="count">количество</param>
        /// <param name="perPage">на странице</param>
        /// <returns></returns>
        public static int PagesCount(int count, int perPage)
        {
            if (count % perPage != 0)
            {
                return (int)Math.Floor((decimal)(count / perPage)) + 1;
            }
            else
            {
                return count / perPage;
            }
        }
    }
}