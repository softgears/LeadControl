using System.Linq;
using System.Reflection;

namespace LeadControl.Domain.Enums
{
    /// <summary>
    /// Вспомогательный метод для работы с перечислениями
    /// </summary>
    public static class EnumUtils
    {
        /// <summary>
        /// Извлекает атрибут Enum Description из члена перечисления для указанной локали
        /// </summary>
        /// <typeparam name="TEnum">Тип перечисления</typeparam>
        /// <param name="enumValue">Элемент из перечисления</param>
        /// <param name="locale">Локаль, по умолчанию ru-RU</param>
        /// <returns>Строка</returns>
        public static string GetEnumMemberName<TEnum>(this TEnum enumValue, string locale = "ru-RU")
        {
            var ti = enumValue.GetType();
            var result = enumValue.ToString();
            foreach (var attr in from fieldInfo in ti.GetFields(BindingFlags.Public | BindingFlags.Static)
                                 where fieldInfo.Name.Equals(enumValue.ToString())
                                 select (EnumDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), false)
                                     into attrs
                                     where attrs.Length > 0
                                     select attrs.FirstOrDefault(r => r.Locale == locale))
            {
                result = attr.Name;
            }
            return result;
        }
    }
}