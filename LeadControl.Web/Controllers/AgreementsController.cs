using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Enums;
using LeadControl.Domain.Routing;
using LeadControl.Web.Classes.Security;
using LeadControl.Web.Models.Agreements;

namespace LeadControl.Web.Controllers
{
    /// <summary>
    /// Контроллер управления договорами
    /// </summary>
    public class AgreementsController : BaseController
    {
        /// <summary>
        /// Отображает список договоров
        /// </summary>
        /// <returns></returns>
        [Route("agreements")][AuthorizationCheck(Permission.ServicesManager)]
        public ActionResult Index(AgreementsFiltrationModel model)
        {
            // Выбираем
            var projects = CurrentUser.IsAdmin() ? DataContext.Projects.Select(p => p.Id) : CurrentUser.ProjectUsers.Select(p => p.ProjectId);
            IEnumerable<LeadAgreement> agreements = DataContext.LeadAgreements.Where(o => projects.Contains(o.ProjectId));
            if (model.LeadIds.Length > 0)
            {
                agreements = agreements.Where(o => model.LeadIds.Contains(o.LeadId));
            }
            if (model.ProjectIds.Length > 0)
            {
                agreements = agreements.Where(o => model.ProjectIds.Contains(o.ProjectId));
            }
            if (model.Statuses.Length > 0)
            {
                agreements = agreements.Where(o => model.Statuses.Contains(o.Status));
            }
            if (model.Managers.Length > 0)
            {
                agreements = agreements.Where(o => model.Managers.Contains(o.AssignedUserId));
            }
            if (!String.IsNullOrEmpty(model.Term))
            {
                var term = model.Term.ToLower();
                agreements =
                    agreements.Where(
                        o =>
                            (o.Title != null && o.Title.ToLower().Contains(term) || (o.Description != null && o.Description.ToLower().Contains(term))));
            }

            model.Fetched = agreements.OrderByDescending(a => a.DateModified).ToList();

            PushNavigationItem("Договора", "/agreements");
            PushNavigationItem("Список договоров", "#");

            return View(model);
        }

        /// <summary>
        /// Отображает страницу создания нового договора
        /// </summary>
        /// <returns></returns>
        [Route("agreements/create")]
        [AuthorizationCheck(Permission.ServicesManager)]
        public ActionResult Create()
        {
            PushNavigationItem("Договора", "/agreements");
            PushNavigationItem("Создание нового договора. Шаг 1", "#");

            return View();
        }

        /// <summary>
        /// Делает проверку уникальности номера договора в системе
        /// </summary>
        /// <param name="Number">Номер договора</param>
        /// <returns></returns>
        [Route("agreements/check-number")][AuthorizationCheck(Permission.ServicesManager)]
        public ActionResult CheckNumber(string Number)
        {
            if (String.IsNullOrEmpty(Number))
            {
                Number = "";
            }
            var exists = DataContext.LeadAgreements.Any(a => a.Number.ToLower() == Number.ToLower());
            return Content(exists ? "Такой номер договора уже существует в системе" : "true");
        }

        /// <summary>
        /// Обрабатывает создание нового договора
        /// </summary>
        /// <param name="Number">Номер договора</param>
        /// <param name="Date">Дата заключения</param>
        /// <param name="DateEnd">Дата истечения</param>
        /// <param name="Title">Название</param>
        /// <param name="Description">Описание</param>
        /// <param name="ProjectId">Проект</param>
        /// <param name="LeadId">Лид</param>
        /// <param name="model">Модель данных нового лида</param>
        /// <returns></returns>
        [HttpPost][Route("agreements/create-1")][AuthorizationCheck(Permission.ServicesManager)]
        public ActionResult Create1Step(string Number, DateTime Date, DateTime? DateEnd, string Title,
            string Description, long ProjectId, long LeadId, Lead model)
        {
            // Ищем или создаем лида, к которому создается заказ
            Lead lead;
            if (LeadId != -1)
            {
                lead = DataContext.Leads.FirstOrDefault(l => l.Id == LeadId);
                if (lead == null)
                {
                    ShowError("Такой лид не найден");
                    return RedirectToAction("Create");
                }
            }
            else
            {
                model.DateCreated = DateTime.Now;
                DataContext.Leads.InsertOnSubmit(model);
                lead = model;
            }

            // Создаем договор
            var agreement = new LeadAgreement()
            {
                Number = Number,
                Date = Date,
                EndDate = DateEnd,
                Title = Title,
                Description = Description,
                ProjectId = ProjectId,
                Lead = lead,
                User = CurrentUser,
                Status = (short) LeadAgreementStatus.Negotiation,
                DateCreated = DateTime.Now
            };
            agreement.LeadAgreementChangements.Add(new LeadAgreementChangement()
            {
                User = CurrentUser,
                LeadAgreement = agreement,
                Comments = "Создание договора пользователем "+CurrentUser.GetFio(),
                DateCreated = DateTime.Now
            });

            DataContext.LeadAgreements.InsertOnSubmit(agreement);
            DataContext.SubmitChanges();

            ShowSuccess(string.Format("Договор №{0} для лида {1} успешно создан", Number, lead.ToString()));

            return RedirectToAction("EditAgreementServices", new {id = agreement.Id});
        }


    }
}
