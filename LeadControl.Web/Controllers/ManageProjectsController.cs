using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Routing;
using LeadControl.Web.Classes.Security;

namespace LeadControl.Web.Controllers
{
    /// <summary>
    /// Контроллер управления проектами-нишами в системе
    /// </summary>
    public class ManageProjectsController : BaseController
    {
        /// <summary>
        /// Отображает список всех ниш в системе
        /// </summary>
        /// <returns></returns>
        [Route("manage/projects")][AuthorizationCheck(Permission.ManageProjects)]
        public ActionResult Index()
        {
            // Репозиторий
            var projects = DataContext.Projects.ToList();

            PushNavigationItem("Проекты", "/manage/projects");
            PushNavigationItem("Список проектов", "#");

            return View(projects);
        }

        /// <summary>
        /// Отображает форму создания нового проекта
        /// </summary>
        /// <returns></returns>
        [Route("manage/projects/add")]
        [AuthorizationCheck(Permission.ManageProjects)]
        public ActionResult Add()
        {
            PushNavigationItem("Роли", "/manage/projects");
            PushNavigationItem("Новый проект", "#");

            ViewBag.users = DataContext.Users.ToList();
            return View("EditProject", new Project());
        }

        /// <summary>
        /// Отображает форму редактирования существующего проекта
        /// </summary>
        /// <param name="id">Идентификатор роли</param>
        /// <returns></returns>
        [Route("manage/projects/{id}/edit")]
        [AuthorizationCheck(Permission.ManageProjects)]
        public ActionResult Edit(long id)
        {
            // Репозиторий
            var project = DataContext.Projects.FirstOrDefault(r => r.Id == id);
            if (project == null)
            {
                ShowError("Такой проект не найден");
                return RedirectToAction("Index");
            }

            PushNavigationItem("Проекты", "/manage/projects");
            PushNavigationItem("Редактирование проекта " + project.Title, "#");

            ViewBag.users = DataContext.Users.ToList();
            return View("EditProject", project);
        }

        /// <summary>
        /// Удаляет указанную роль если в ней нет пользователей
        /// </summary>
        /// <param name="id">Идентификатор роли</param>
        /// <returns></returns>
        [Route("manage/projects/{id}/delete")]
        [AuthorizationCheck(Permission.ManageProjects)]
        public ActionResult Delete(long id)
        {
            // Репозиторий
            var project = DataContext.Projects.FirstOrDefault(r => r.Id == id);
            if (project == null)
            {
                ShowError("Такой проект не найден");
                return RedirectToAction("Index");
            }

            if (project.LeadOrders.Count > 0)
            {
                ShowError("Нельзя удалить этот проект, потому что в нем имеются заказы");
                return RedirectToAction("Index");
            }
            if (project.LeadAgreements.Count > 0)
            {
                ShowError("Нельзя удалить этот проект, потому что в нем имеются договора на оказание услуг");
                return RedirectToAction("Index");
            }

            DataContext.Projects.DeleteOnSubmit(project);
            DataContext.SubmitChanges();

            ShowSuccess("Проект успешно удален");

            return RedirectToAction("Index");
        }



        /// <summary>
        /// Сохраняет изменения в существуюющем проекте или создает новый проект
        /// </summary>
        /// <param name="model">Модель основных данных роли</param>
        /// <param name="collection">Коллекция</param>
        /// <returns></returns>
        [Route("manage/projects/save")]
        [AuthorizationCheck(Permission.ManageProjects)]
        [HttpPost]
        public ActionResult Save(Project model, FormCollection collection)
        {
            // Репозиторий
            Project project;
            if (model.Id <= 0)
            {
                project = model;
                project.DateCreated = DateTime.Now;

                DataContext.Projects.InsertOnSubmit(project);
                DataContext.SubmitChanges();

                ShowSuccess("Проект успешно создан");
            }
            else
            {
                project = DataContext.Projects.FirstOrDefault(r => r.Id == model.Id);
                if (project == null)
                {
                    ShowError("Такой проект не найден");
                    return RedirectToAction("Index");
                }

                project.Title = model.Title;
                project.Description = model.Description;
                project.Url = model.Url;
                project.DateModified = DateTime.Now;

                DataContext.SubmitChanges();
                ShowSuccess("Проект успешно отредактирован");
            }

            // Обновляем пермишенны для роли
            project.ProjectUsers.Clear();
            foreach (var userId in collection.AllKeys.Where(k => k.StartsWith("user_")).Select(key => Convert.ToInt64(key.Split('_')[1])))
            {
                project.ProjectUsers.Add(new ProjectUser()
                {
                    Project = project,
                    UserId = userId,
                    DateCreated = DateTime.Now
                });
            }
            DataContext.SubmitChanges();

            return RedirectToAction("Index");
        }

    }
}
