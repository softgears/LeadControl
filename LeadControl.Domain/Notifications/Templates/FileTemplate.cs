using System.IO;

namespace LeadControl.Domain.Notifications.Templates
{
    /// <summary>
    /// Файловый шаблон, который грузит свое содержимое из файла
    /// </summary>
    public class FileTemplate: BaseTemplate
    {
        /// <summary>
        /// Стандартный конструктор, который загружает имя файла из шаблона
        /// </summary>
        /// <param name="fileName">Путь к файлу шаблона</param>
        public FileTemplate(string fileName)
        {
            Content = File.OpenText(fileName).ReadToEnd();
        }
    }
}