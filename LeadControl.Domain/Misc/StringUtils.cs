using System.Linq;
using System.Text;

namespace LeadControl.Domain.Misc
{
    /// <summary>
    /// Статический класс содержащий утилитарные методы для работ со строками
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Генерирует строку, состояющую из указанного символа в указанном количестве
        /// </summary>
        /// <param name="character">Символ</param>
        /// <param name="count">Количество</param>
        /// <returns>Цельная строка</returns>
        public static string GenerateString(char character, int count)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < count; i++)
            {
                sb.Append(character);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Нормализует строку, содержащую номер телефона
        /// </summary>
        /// <param name="phone">Номер телефона</param>
        /// <returns>Нормализованный номер телефона</returns>
        public static string NormalizePhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return "";
            }
            var str = new StringBuilder(phone);
            str.Replace("-", string.Empty).Replace(")", string.Empty).Replace("(", string.Empty).Replace("+7", "7").Replace(" ","");
            if (phone.StartsWith("8"))
            {
                str[0] = '7';
            }
            return str.ToString();
        }

        /// <summary>
        /// Форматирует номер телефона для удобного отображения на сайте
        /// </summary>
        /// <param name="phone">Номер телефона</param>
        /// <returns></returns>
        public static string FormatPhoneNumber(this string phone)
        {
            var ph = NormalizePhoneNumber(phone);
            var sb = new StringBuilder(ph);
            if (sb.Length == 11)
            {
                return sb.Insert(0, '+').Insert(2, '-').Insert(6, '-').Insert(10, '-').Insert(13, '-').ToString();
            }
            else if (sb.Length == 6)
            {
                return sb.Insert(2, "-").Insert(5, "-").ToString();
            }
            else return ph;
        }

        /// <summary>
        /// Форматирует число как цену, разбивая его на разряды
        /// </summary>
        /// <param name="price">Цена</param>
        /// <returns></returns>
        public static string FormatPrice(this double? price)
        {
            if (price == null)
            {
                return string.Empty;
            }
            return string.Format("{0:N}", (long)price.Value).Replace(",00","");
        }

        /// <summary>
        /// Возвращает отформатированную строку с размером файла с мегабайтами
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string FormatFileSize(this long size)
        {
            if (size > (1024 * 1024 * 1024))
            {
                double s = size, d = 1073741824.0;
                return string.Format("{0:0.0} Гб", s / d);
            }
            if (size > (1024*1024))
            {
                double s = size, d = 1048576.0;
                return string.Format("{0:0.0} Мб", s / d);
            }
            if (size > 1024)
            {
                double s = size, d = 1024;
                return string.Format("{0:0.0} Кб", s / d);
            }
            return string.Format("{0:0} байт", size);
        }

        /// <summary>
        /// Выполняет транслитерацию текста
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Transliterate(this string str)
        {
            return Transliteration.Front(str, TransliterationType.ISO);
        }

        /// <summary>
        /// Выдирает доменную зону из имени домена
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ExtractZone(this string str)
        {
            var parts = str.Split('.').ToList();
            parts.RemoveAt(0);
            return string.Join(".", parts);
        }
    }
}