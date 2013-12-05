using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeadControl.Domain.DAL;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Interfaces.Notifications;
using LeadControl.Domain.IoC;
using LeadControl.Web.Classes.Navigation;

namespace LeadControl.Web.Controllers
{
    /// <summary>
    /// Базовый контроллер системы
    /// </summary>
    public  abstract class BaseController : Controller
    {
        /// <summary>
        /// Контекст доступа к данным 
        /// </summary>
        public LCDataContext DataContext { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Mvc.Controller"/> class.
        /// </summary>
        protected BaseController()
        {
            DataContext = Locator.GetService<LCDataContext>();
        }

        #region сессии

        /// <summary>
        /// Хранение текущего пользователя
        /// </summary>
        private User _user { get; set; }

        /// <summary>
        /// Текущий авторизованный пользователь
        /// </summary>
        public User CurrentUser
        {
            get
            {
                object fromSess = Session["CurrentUser"];
                if (fromSess == null)
                {
                    return null;
                }
                var userId = (long)fromSess;
                if (_user == null)
                {
                    _user = DataContext.Users.FirstOrDefault(u => u.Id == userId);
                }
                return _user;
            }
            set
            {
                Session["CurrentUser"] = value != null ? (object)value.Id : null;
                _user = value;
            }
        }

        /// <summary>
        /// Является ли текущий пользователь авторизованным
        /// </summary>
        public bool IsAuthentificated
        {
            get { return CurrentUser != null; }
        }

        /// <summary>
        /// Авторизирует текущего пользователя
        /// </summary>
        /// <param name="user">Пользователь которого установить как текущего</param>
        /// <param name="remember">Запомнить ли пользователя</param>
        public void AuthorizeUser(User user, bool remember = true)
        {
            CurrentUser = user;
            if (remember)
            {
                // Устанавливаем собственные авторизационные куки
                var authCookie = new HttpCookie("auth");
                authCookie.Values["identity"] = user.Email;
                authCookie.Values["pass"] = user.PasswordHash;
                authCookie.Expires = DateTime.Now.AddDays(365);
                Response.Cookies.Add(authCookie);
            }

        }

        /// <summary>
        /// Убирает авторизацию текущего пользователя и убирает авторизационные куки если они есть
        /// </summary>
        public void CloseAuthorization()
        {
            CurrentUser = null;

            // убираем куки если они есть
            var authCookie = Response.Cookies["auth"];
            if (authCookie != null)
            {
                authCookie = new HttpCookie("auth")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(authCookie);
            }
        }

        #endregion

        #region навигация

        /// <summary>
        /// Коллекция навигционных жлементов
        /// </summary>
        public IList<NavigationChainItem> NavigationChain { get; set; }

        /// <summary>
        /// Добавляет новый элемент в навигационную цепочку
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        public void PushNavigationItem(string title, string url)
        {
            if (NavigationChain == null)
            {
                NavigationChain = new List<NavigationChainItem>();
            }
            NavigationChain.Add(new NavigationChainItem()
            {
                Title = title,
                Url = url
            });
        }

        #endregion

        #region Уведомления

        /// <summary>
        /// Отображает сообщение об ошибке
        /// </summary>
        /// <param name="message">Сообщение</param>
        public void ShowError(string message)
        {
            Locator.GetService<IUINotificationManager>().Error(message);
        }

        /// <summary>
        /// Отображает сообщение об успешном выполнении операции
        /// </summary>
        /// <param name="message"></param>
        public void ShowSuccess(string message)
        {
            Locator.GetService<IUINotificationManager>().Success(message);
        }

        /// <summary>
        /// Уведомляем указанного пользователя Email сообщением
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="subject">Тема сообщения</param>
        /// <param name="content">Содержимое</param>
        public void NotifyEmail(User user, string subject, string content)
        {
            Locator.GetService<IMailNotificationManager>().Notify(user, subject, content);
        }

        #endregion
    }
}
