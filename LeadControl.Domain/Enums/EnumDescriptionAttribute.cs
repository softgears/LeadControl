using System;

namespace LeadControl.Domain.Enums
{
    /// <summary>
    /// Аттрибут помечающий элемент перечисления с помощью строк
    /// </summary>
    public class EnumDescriptionAttribute: Attribute
    {
        /// <summary>
        /// Название элемента в перечислении
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Локаль с которой данное имя должно использоваться
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Создает новый атрибут помечающий перечисление
        /// </summary>
        /// <param name="name"></param>
        public EnumDescriptionAttribute(string name)
        {
            Name = name;
            Locale = "ru-RU";
        }
    }
}