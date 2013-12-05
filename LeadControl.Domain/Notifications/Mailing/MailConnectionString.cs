using System;

namespace LeadControl.Domain.Notifications.Mailing
{
    /// <summary>
    /// Строка подключения, используемая для рассылки электронной почты
    /// </summary>
    public class MailConnectionString
    {
        /// <summary>
        /// Адрес подключения
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Порт подключения
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Адрес отправителя
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// Имя отправителя
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Использовать ли SSL для подключения
        /// </summary>
        public bool UseSsl { get; set; }

        /// <summary>
        /// Логин для подключения
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль для авторизации
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Создает строку подключения
        /// </summary>
        /// <param name="mailConnectionString">Строка подключения в виде строки</param>
        public MailConnectionString(string mailConnectionString)
        {
            // Парсит строку разбивая все составляющие по ;
            var parts = mailConnectionString.Split(';');
            foreach (var part in parts)
            {
                // Разбиваем имеющиеся части по =
                var subParts = part.Split('=');
                if (subParts.Length != 2)
                {
                    continue;
                }
                switch (subParts[0].ToLower().Trim())
                {
                    case "host":
                        Host = subParts[1];
                        break;
                    case "port":
                        Port = Convert.ToInt32(subParts[1]);
                        break;
                    case "fromaddress":
                        FromAddress = subParts[1];
                        break;
                    case "fromname":
                        FromName = subParts[1];
                        break;
                    case "login":
                        Login = subParts[1];
                        break;
                    case "password":
                        Password = subParts[1];
                        break;
                    case "usessl":
                        UseSsl = Convert.ToBoolean(subParts[1]);
                        break;
                }
            }
        }

        /// <summary>
        /// Возвращает строковое представление текущего объекта
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Host={0};Port={1};Login={2};Password={3};UseSsl={4};FromAddress={5};FromName={6}",Host,Port,Login,Password,UseSsl,FromAddress,FromName);
        }
    }
}