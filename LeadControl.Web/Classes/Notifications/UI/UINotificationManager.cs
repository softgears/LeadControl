using System.Collections.Generic;
using System.Web;
using LeadControl.Domain.Interfaces.Notifications;

namespace LeadControl.Web.Classes.Notifications.UI
{
    /// <summary>
    /// Нотификатор через пользовательский интерфейс
    /// </summary>
    public class UINotificationManager: IUINotificationManager
    {
        /// <summary>
        /// Добавляет сообщение в стек нотификаций текущему пользователю
        /// </summary>
        /// <param name="notificationItem">Объект нотификации</param>
        private void PushNotificationItem(UINotificationItem notificationItem)
        {
            var notificationStack = GetOrCreateNotificationStack();

            // Помещаем новый элемент в него
            notificationStack.Add(notificationItem);
        }

        /// <summary>
        /// Получает или создает в сессии пользователя стек нотификаций
        /// </summary>
        /// <returns></returns>
        public IList<UINotificationItem> GetOrCreateNotificationStack()
        {
            // Получаем контекст пользователя
            var context = HttpContext.Current;
            if (context == null)
            {
                return null;
            }

            // Получаем или создаем стек сообщений нотифицирования
            var notificationStack = (IList<UINotificationItem>) context.Session["UINotifications"];
            if (notificationStack == null)
            {
                notificationStack = new List<UINotificationItem>();
                context.Session["UINotifications"] = notificationStack;
            }
            return notificationStack;
        }

        /// <summary>
        /// Очищает стек уведомлений у текущего пользователя
        /// </summary>
        public void ClearNotificationStack()
        {
            // Получаем контекст пользователя
            var context = HttpContext.Current;
            if (context != null)
            {
                context.Session.Remove("UINotifications");
            }
        }

        /// <summary>
        /// Нотифицирует текущего пользователя об успешном выполнении операции
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        public void Success(string message)
        {
            PushNotificationItem(new UINotificationItem(NotificationItemType.Success, message));
        }

        /// <summary>
        /// Нотифицирует текущего пользователя сообщением об неуспешном выполнении операции
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            PushNotificationItem(new UINotificationItem(NotificationItemType.Error, message));
        }

        /// <summary>
        /// Нотифицирует текущего пользователя предупреждением
        /// </summary>
        /// <param name="message"></param>
        public void Warning(string message)
        {
            PushNotificationItem(new UINotificationItem(NotificationItemType.Warning, message));
        }
    }
}