using System;
using System.Text;
using System.Text.RegularExpressions;

namespace LeadControl.Domain.Notifications.Templates
{
    /// <summary>
    /// Файл шаблона, который загружается из файла и формирует свое содержимое с помощью
    /// </summary>
    public class ParametrizedFileTemplate: FileTemplate
    {
        /// <summary>
        /// Модель, используемая
        /// </summary>
        public object Model { get; private set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        /// <param name="fileName">Имя и путь к файлу</param>
        /// <param name="model">Модель данных</param>
        public ParametrizedFileTemplate(string fileName, object model) : base(fileName)
        {
            Model = model;
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
                var value = propertyInfo.GetValue(Model,null);

                // Выполняем замену
                sb.Replace(match.Value, value != null ? value.ToString() : String.Empty);
            }

            // Отдаем преобразованный результат
            return sb.ToString();
        }
    }
}