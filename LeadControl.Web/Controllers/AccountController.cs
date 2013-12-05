using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeadControl.Domain.Interfaces.Notifications;
using LeadControl.Domain.IoC;
using LeadControl.Domain.Misc;
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
    }
}
