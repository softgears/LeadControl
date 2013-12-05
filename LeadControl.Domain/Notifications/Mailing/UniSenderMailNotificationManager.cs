using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using LeadControl.Domain.DAL;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Interfaces.Notifications;
using LeadControl.Domain.IoC;
using NLog;

namespace LeadControl.Domain.Notifications.Mailing
{
    /// <summary>
    /// Менеджер нотификаций по Email, использующий UniSender
    /// </summary>
    public class UniSenderMailNotificationManager: IMailNotificationManager
    {
        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        public UniSenderMailNotificationManager()
        {
            ProcessingTimer = new Timer(state =>
            {
                if (!ProcessingActive)
                {
                    try
                    {
                        ProcessingActive = true;
                        FlushQueue();
                        ProcessingActive = false;
                    }
                    catch (Exception e)
                    {
                        ProcessingActive = false;
                        Logger.Error(String.Format("Ошибка в ходе обработки очереди сообщений: {0}", e.Message));
                    }
                }

            }, null, TimerPeriod, TimerPeriod);
            Logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Логгер текущего класса
        /// </summary>
        private Logger Logger { get; set; }

        /// <summary>
        /// Период срабатывания таймера
        /// </summary>
        private const long TimerPeriod = 10000;

        /// <summary>
        /// Флаг, указывающий, находится ли очередь в процессе обработки
        /// </summary>
        private bool ProcessingActive { get; set; }

        /// <summary>
        /// Таймер, выполняющий обработку писем по времени
        /// </summary>
        private System.Threading.Timer ProcessingTimer { get; set; }

        /// <summary>
        /// Обрабатывает очередь сообщений
        /// </summary>
        private void FlushQueue()
        {
            using (var httpRequestScope = Locator.BeginNestedHttpRequestScope())
            {
                // Репозиторий
                var dc = Locator.GetService<LCDataContext>();
                var messages = dc.MailNotificationMessages.Where(m => m.Sended == false).ToList();
                var uniSenderApiKey = System.Configuration.ConfigurationManager.AppSettings["UniSenderApiKey"];

                // Выходим если в очереди пусто
                if (messages.Count == 0)
                {
                    return;
                }

                // Получаем данные для отправки
                var connectionData = new MailConnectionString(System.Configuration.ConfigurationManager.AppSettings["MailConnectionString"]);

                // Обрабатываем очередь
                Logger.Info(string.Format("Обрабатываем очередь сообщений, в очереди {0} писем", messages.Count));
                var sendedCount = 0;
                foreach (var msg in messages)
                {
                    var client = new WebClient();
                    var request = "http://api.unisender.com/ru/api/sendEmail?format=json";
                    var requestParams = new NameValueCollection();
                    requestParams.Add("api_key", uniSenderApiKey);
                    requestParams.Add("email", msg.Recipient);
                    requestParams.Add("sender_name", "Trust-Media Lead Control CRM System");
                    requestParams.Add("sender_email", "mailing.nprdv@gmail.com");
                    requestParams.Add("subject", msg.Subject);
                    requestParams.Add("body", msg.Content);
                    requestParams.Add("list_id", "2619464");
                    requestParams.Add("lang", "ru");
                    

                    // Пытаемся отправить
                    try
                    {
                        var response = Encoding.UTF8.GetString(client.UploadValues(request, requestParams));
                        if (response.Contains("\"email_id\""))
                        {
                            sendedCount++;    
                        }
                        else
                        {
                            throw new Exception("Не удалось отправить сообщение т.к. UniSender не вернул его id");
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(string.Format("Не удалось отправить письмо получателю {0} по причине: {1}", msg.Recipient, e.Message));
                        continue;
                    }

                    // Помечаем письмо как отправленное
                    msg.Sended = true;
                    msg.DateSended = DateTime.Now;
                    dc.SubmitChanges();
                }

                Logger.Info(string.Format("Обработка очереди сообщений завершена. Отправлено {0} писем из {1}", sendedCount, messages.Count));    
            }
        }

        /// <summary>
        /// Уведомляет указанного пользователя посредством Email сообщения
        /// </summary>
        /// <param name="user"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public void Notify(User user, string title, string content)
        {
            Notify(user.Email,title,content);
        }

        public void Notify(string mailto, string title, string content)
        {
            using (var httpRequestScope = Locator.BeginNestedHttpRequestScope())
            {
                // Создаем сообщение и помещаем его в очередь
                var dc = Locator.GetService<LCDataContext>();
                var newMessage = new MailNotificationMessage()
                {
                    Recipient = mailto,
                    Subject = title,
                    Content = content,
                    DateEnqued = DateTime.Now,
                    Sended = false
                };
                dc.MailNotificationMessages.InsertOnSubmit(newMessage);
                dc.SubmitChanges();
            }
        }

        /// <summary>
        /// Инициализирует менеджер
        /// </summary>
        public void Init()
        {
            
        }
    }
}