using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeadControl.Domain.Entities;
using LeadControl.Domain.IoC;
using LeadControl.Domain.Misc;
using LeadControl.Domain.Notifications.Templates;
using LeadControl.Domain.Routing;
using LeadControl.Web.Classes.Security;
using LeadControl.Web.Models.Manage;

namespace LeadControl.Web.Controllers
{
    /// <summary>
    /// Контроллер управления пользователями
    /// </summary>
    public class ManageUsersController : BaseController
    {
        /// <summary>
        /// Отображает фильтруемый список пользователей
        /// </summary>
        /// <returns></returns>
        [Route("manage/users")][AuthorizationCheck(Permission.ManageUsers)]
        public ActionResult Index(UsersFiltrationModel model)
        {
            PushNavigationItem("Управление пользователями","/manage/users");
            PushNavigationItem("Список пользователей","#");

            // Фильтруем
            IEnumerable<User> users = DataContext.Users;
            if (model.RoleIds.Length > 0)
            {
                users = users.Where(u => model.RoleIds.Contains(u.RoleId));
            }
            if (!String.IsNullOrEmpty(model.Term))
            {
                var term = model.Term.ToLower();
                users = users.Where(u => (u.FirstName != null && u.FirstName.ToLower().Contains(term)) ||
                                         (u.SurName != null && u.SurName.ToLower().Contains(term)) ||
                                         (u.LastName != null && u.LastName.ToLower().Contains(term)) ||
                                         (u.Email != null && u.Email.ToLower().Contains(term)));
            }

            model.Fetched = users.ToList();

            return View(model);
        }

        /// <summary>
        /// Отображает форму создания нового пользователя
        /// </summary>
        /// <returns></returns>
        [Route("manage/users/add")][AuthorizationCheck(Permission.ManageUsers)]
        public ActionResult CreateUser()
        {
            PushNavigationItem("Управление пользователями", "/manage/users");
            PushNavigationItem("Создание пользователя", "#");

            return View();
        }

        /// <summary>
        /// Обрабатывает создание нового пользователя через личный кабинет администратора
        /// </summary>
        /// <param name="user">Модель основных данных пользователя</param>
        /// <param name="Password">Пароль</param>
        /// <param name="PasswordConfirm">Подтверждение пароля</param>
        /// <returns></returns>
        [Route("manage/users/create")]
        [HttpPost]
        [AuthorizationCheck(Permission.ManageUsers)]
        public ActionResult Create(User user, string Password, string PasswordConfirm)
        {
            // Реп
            if (DataContext.Users.Any(u => u.Email.ToLower() == user.Email.ToLower()))
            {
                ShowError("Такой пользователь уже зарегистрирован");
                return RedirectToAction("Index");
            }

            // Регистрируем
            user.PasswordHash = PasswordUtils.GenerateMD5PasswordHash(Password);
            user.DateRegistred = DateTime.Now;

            DataContext.Users.InsertOnSubmit(user);
            DataContext.SubmitChanges();

            ShowSuccess(string.Format("Успешно зарегистрирован пользователь {0} {1}. На его почту отправлено письмо с информацией о регистрации", user.GetFio(), user.Email));

            // Уведомляем
            var notificationModel = new
            {
                FIO = user.GetFio(),
                Email = user.Email,
                Password = Password
            };

            NotifyEmail(user, "Регистрация в системе LeadControl",
                new ParametrizedFileTemplate(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Mail", "Register.html"),
                    notificationModel).ToString());

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Карточка пользователя с ограниченной инфорацией=
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [AuthorizationCheck(Permission.ManageUsers)]
        [Route("manage/users/{id}/info")]
        public ActionResult Info(long id)
        {
            var user = DataContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                ShowError("Такой пользователь не найден");
                return RedirectToAction("Index");
            }

            PushNavigationItem("Управление пользователями", "/manage/users");
            PushNavigationItem("Карточка пользователя " + user.GetFio(), "#");

            return View(user);
        }

        /// <summary>
        /// Обновляет некоторые данные о пользователе
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="RoleId">Идентификатор оли</param>
        /// <param name="Status">Статус пользователя</param>
        /// <returns></returns>
        [AuthorizationCheck(Permission.ManageUsers)]
        [Route("manage/users/update-info")]
        [HttpPost]
        public ActionResult UpdateCard(long id, long RoleId, short Status)
        {
            var user = DataContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                ShowError("Такой пользователь не найден");
                return RedirectToAction("Index");
            }

            
            var role = DataContext.Roles.FirstOrDefault(r => r.Id == RoleId);

            if (RoleId != user.RoleId)
            {
                user.Role.Users.Remove(user);
                role.Users.Add(user);
            }
            user.Status = Status;

            DataContext.SubmitChanges();

            ShowSuccess("Изменения успешно сохранены");

            return RedirectToAction("Index");
        }

    }
}
