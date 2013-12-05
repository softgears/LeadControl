using LeadControl.Domain.Entities;

namespace LeadControl.Domain.Interfaces.Notifications
{
    /// <summary>
    /// Менеджер нотификаций по СМС сообщениям
    /// </summary>
    public interface ISMSNotificationManager
    {
        /// <summary>
        /// Нотифицирует указанный номер телефона указанным сообщением смс
        /// </summary>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <param name="message">Сообщение</param>
        void Notify(string phoneNumber, string message);

        /// <summary>
        /// Нотифицирует указанного пользователя указанным сообщением по смс
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="message">сообщение</param>
        void Notify(User user, string message);

        /// <summary>
        /// Инициализирует менеджер
        /// </summary>
        void Init();
    }
}