namespace LeadControl.Web.Classes.Notifications.UI
{
    /// <summary>
    /// Элемент в стеке нотификаций
    /// </summary>
    public class UINotificationItem
    {
        /// <summary>
        /// Тип элемента нотификации: ошибка, успех, предупреждение
        /// </summary>
        public NotificationItemType ItemType { get; set; }

        /// <summary>
        /// Сообщение нотификации
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        /// <param name="itemType">Тип нотификации</param>
        /// <param name="message">Сообщение</param>
        public UINotificationItem(NotificationItemType itemType, string message)
        {
            ItemType = itemType;
            Message = message;
        }
    }
}