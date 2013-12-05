using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Interfaces.Notifications;
using LeadControl.Domain.IoC;
using LeadControl.Domain.Misc;
using LeadControl.Domain.Routing;
using LeadControl.Web.Classes.Security;
using LeadControl.Web.Models.Account;

namespace LeadControl.Web.Controllers
{
    /// <summary>
    /// Контроллер управления пользователя
    /// </summary>
    public class AccountController : BaseController
    {
        /// <summary>
        /// Отображает страницу логина в систему
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            if (IsAuthentificated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        /// <summary>
        /// Обрабатывает авторизацию на сайте
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (IsAuthentificated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            // Валидируем что пользоатель есть
            var user = DataContext.Users.FirstOrDefault(u => u.Email.ToLower() == model.Email.ToLower() && u.PasswordHash == PasswordUtils.GenerateMD5PasswordHash(model.Password));
            if (user == null)
            {
                Locator.GetService<IUINotificationManager>().Error("Такой пользователь не найден");
                return View(model);
            }

            // Авторизуем
            AuthorizeUser(user, model.RememberMe);
            user.LastLogin = DateTime.Now;
            DataContext.SubmitChanges();

            // Идем в личный кабинет
            return RedirectToAction("Index", "Dashboard");
        }

        /// <summary>
        /// Выполняет закрытие текущий сессии пользователя и выход из личного кабинета
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Login");
            }

            CloseAuthorization();
            return RedirectToAction("Login");
        }

        #region Профиль пользователя

        /// <summary>
        /// Отображает форму редактирования пользовательского профиля
        /// </summary>
        /// <returns></returns>
        [Route("account/profile")][AuthorizationCheck]
        public ActionResult EditProfile()
        {
            PushNavigationItem("Профиль","#");

            return View();
        }



        /// <summary>
        /// Обновляет личные данные профиля
        /// </summary>
        /// <param name="model">Модель данных</param>
        /// <returns></returns>
        [AuthorizationCheck]
        [HttpPost]
        [Route("account/profile/update")]
        public ActionResult UpdateProfile(User model)
        {
            CurrentUser.FirstName = model.FirstName;
            CurrentUser.LastName = model.LastName;
            CurrentUser.SurName = model.SurName;
            CurrentUser.Phone = model.Phone;
            DataContext.SubmitChanges();

            ShowSuccess("Профиль был успешно сохранен");

            return RedirectToAction("Profile");
        }

        #endregion
    }
}
