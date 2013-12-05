// ============================================================
// 
// 	Astramed
// 	SoftGears.CMS.Infrastructure 
// 	ParametrizedTemplate.cs
// 
// 	Created by: ykorshev 
// 	 at 04.07.2013 12:37
// 
// ============================================================

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace LeadControl.Domain.Notifications.Templates
{
    /// <summary>
    /// Параметризованный шаблон
    /// </summary>
    public class ParametrizedTemplate: BaseTemplate
    {
        /// <summary>
        /// Модель
        /// </summary>
        public object Model { get; set; }

        /// <summary>
        /// Инициаилизирует параметризованный шаблон
        /// </summary>
        /// <param name="content">Шаблон</param>
        /// <param name="model">Объект</param>
        public ParametrizedTemplate(string content, object model)
        {
            Model = model;
            Content = content;
        }

        /// <summary>
        /// Обрабатывает шаблон, вставляет значение полей моделей в макросы в шаблоне
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        protected override string Process(string content)
        {
            // Подгатавливаем данные
            var parseRegEx = new Regex(@"\$\{([A-Za-z0-9]+?)\}");
            var sb = new StringBuilder(content);

            var ti = Model.GetType();

            // Находим все вхождения макросов по регулярному выражению
            var matches = parseRegEx.Matches(content);
            foreach (Match match in matches)
            {
                var propertyName = match.Groups[1].Value;

                // Ищем свойство у модели
                var propertyInfo = ti.GetProperty(propertyName);
                if (propertyInfo == null)
                {
                    // Похоже что данное свойство у модели не найдено
                    continue;
                }
                var value = propertyInfo.GetValue(Model, null);

                // Выполняем замену
                sb.Replace(match.Value, value != null ? value.ToString() : String.Empty);
            }

            // Отдаем преобразованный результат
            return sb.ToString();
        }
    }
}