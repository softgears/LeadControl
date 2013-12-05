using LeadControl.Domain.Entities;

namespace LeadControl.Domain.Interfaces.Notifications
{
    /// <summary>
    /// Абстрактный нотификатор по электронной почте
    /// </summary>
    public interface IMailNotificationManager
    {
        /// <summary>
        /// Нотифицирует указанного пользователя сообщением по электронной почте
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="title">Заголовок письма</param>
        /// <param name="content">Содержимое письма</param>
        void Notify(User user, string title, string content);

        /// <summary>
        /// Нотифицирует указанный адрес сообщением по электронной почте
        /// </summary>
        /// <param name="mailto">Адрес получателя</param>
        /// <param name="title">Тема письма</param>
        /// <param name="content">Содержимое письма</param>
        void Notify(string mailto, string title, string content);

        /// <summary>
        /// Инициализирует внутренние сервисы нотификации
        /// </summary>
        void Init();
    }
}