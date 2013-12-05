using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using LeadControl.Domain.DAL;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Interfaces.Notifications;
using LeadControl.Domain.IoC;
using NLog;
using NLog.Internal;

namespace LeadControl.Domain.Notifications.Mailing
{
    /// <summary>
    /// Инфраструктурная реализация менеджера нотификаций по электронной почте
    /// </summary>
    public class MailNotificationManager: IMailNotificationManager
    {
        /// <summary>
        /// Логгер текущего класса
        /// </summary>
        private Logger Logger { get; set; }

        /// <summary>
        /// Период срабатывания таймера
        /// </summary>
        private const long TimerPeriod = 5000;

        /// <summary>
        /// Флаг, указывающий, находится ли очередь в процессе обработки
        /// </summary>
        private bool ProcessingActive { get; set; }

        /// <summary>
        /// Таймер, выполняющий обработку писем по времени
        /// </summary>
        private System.Threading.Timer ProcessingTimer { get; set; }

        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        public MailNotificationManager()
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
                                                        Logger.Error(String.Format("Ошибка в ходе обработки очереди сообщений: {0}",e.Message));
                                                    }    
                                                }
                                                
                                            },null,TimerPeriod,TimerPeriod);
            Logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Обрабатывает очередь сообщений
        /// </summary>
        private void FlushQueue()
        {
            var notifications = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["Notifications"]);
            if (!notifications)
            {
                return;
            }
            using (var httpRequestScope = Locator.BeginNestedHttpRequestScope())
            {
                // Репозиторий
                var dc = Locator.GetService<LCDataContext>();
                var messages = dc.MailNotificationMessages.Where(m => m.Sended == false).ToList();

                // Выходим если в очереди пусто
                if (messages.Count == 0)
                {
                    return;
                }

                // Получаем данные для отправки
                var connectionData = new MailConnectionString(System.Configuration.ConfigurationManager.AppSettings["MailConnectionString"]);

                // Подгатавливаем клиент
                var mailClient = new SmtpClient(connectionData.Host, connectionData.Port)
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(connectionData.Login, connectionData.Password),
                    EnableSsl = connectionData.UseSsl
                };

                // Обрабатываем очередь
                Logger.Info(string.Format("Обрабатываем очередь сообщений, в очереди {0} писем", messages.Count));
                var sendedCount = 0;
                foreach (var msg in messages)
                {
                    // Формируем письмо
                    var mailMessage = new MailMessage(new MailAddress(connectionData.FromAddress, connectionData.FromName),
                                                      new MailAddress(msg.Recipient))
                    {
                        Subject = msg.Subject,
                        SubjectEncoding = Encoding.UTF8,
                        Body = msg.Content,
                        BodyEncoding = Encoding.UTF8,
                        IsBodyHtml = true
                    };

                    // Пытаемся отправить
                    try
                    {
                        mailClient.Send(mailMessage);
                        sendedCount++;
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
        /// Нотифицирует указанного пользователя сообщением по электронной почте
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="title">Заголовок письма</param>
        /// <param name="content">Содержимое письма</param>
        public void Notify(User user, string title, string content)
        {
            Notify(user.Email,title,content);
        }

        /// <summary>
        /// Нотифицирует указанный адрес сообщением по электронной почте
        /// </summary>
        /// <param name="mailto">Получатель</param>
        /// <param name="title">Тема письма</param>
        /// <param name="content">Содержимое</param>
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
        /// Инициализирует внутренние сервисы нотификации
        /// </summary>
        public void Init()
        {
            // NOTE: просто вызываем конструктор через IoC контейнер
        }
    }
}