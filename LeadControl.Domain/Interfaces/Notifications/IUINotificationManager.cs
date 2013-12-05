namespace LeadControl.Domain.Interfaces.Notifications
{
    /// <summary>
    /// Менелдер нотификаций пользователей через пользовательский интерфейс
    /// </summary>
    public interface IUINotificationManager
    {
        /// <summary>
        /// Нотифицирует текущего пользователя об успешном выполнении операции
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        void Success(string message);

        /// <summary>
        /// Нотифицирует текущего пользователя сообщением об неуспешном выполнении операции
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);

        /// <summary>
        /// Нотифицирует текущего пользователя предупреждением
        /// </summary>
        /// <param name="message"></param>
        void Warning(string message);
    }
}