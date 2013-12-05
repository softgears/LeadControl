namespace LeadControl.Domain.Notifications.Templates
{
    /// <summary>
    /// Базовый шаблон, который используется для подготовки текста писем
    /// </summary>
    public abstract class BaseTemplate
    {
        /// <summary>
        /// Контент шаблона
        /// </summary>
        protected string Content { get; set; }

        /// <summary>
        /// Метод, вызываемый при обработке шаблона
        /// </summary>
        /// <param name="content">Контент шаблона</param>
        protected virtual string Process(string content)
        {
            return content;
        }

        /// <summary>
        /// Обрабатывает шаблон и возвращает его строковое представление
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Process(Content);
        }
    }
}