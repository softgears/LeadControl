using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Web.Mvc;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Routing;
using LeadControl.Web.Classes.Security;
using LeadControl.Web.Models.Leads;

namespace LeadControl.Web.Controllers
{
    /// <summary>
    /// Контроллер управления лидами
    /// </summary>
    public class LeadsController : BaseController
    {
        /// <summary>
        /// Отображает список лидов
        /// </summary>
        /// <returns></returns>
        [Route("leads")][AuthorizationCheck()]
        public ActionResult Index(LeadsFiltrationModel model)
        {
            PushNavigationItem("Лиды","/leads");
            PushNavigationItem("Список лидов","");

            // Выбираем
            IEnumerable<Lead> items = DataContext.Leads;
            if (model.ProjectIds.Length > 0)
            {
                items = items.Where(i => i.LeadOrders.Any(lo => model.ProjectIds.Contains(lo.ProjectId)));
            }
            if (!String.IsNullOrEmpty(model.Term))
            {
                var term = model.Term.ToLower();
                items =
                    items.Where(
                        i =>
                            (i.FirstName != null && i.FirstName.ToLower().Contains(term)) ||
                            (i.LastName != null && i.LastName.ToLower().Contains(term)) ||
                            (i.SurName != null && i.SurName.ToLower().Contains(term)) ||
                            (i.Email != null && i.Email.ToLower().Contains(term)) ||
                            (i.LeadLegalInfos != null && i.LeadLegalInfos.CompanyName != null && i.LeadLegalInfos.CompanyName.ToLower().Contains(term)) ||
                            (i.LeadLegalInfos != null && i.LeadLegalInfos.DirectorFIO != null && i.LeadLegalInfos.DirectorFIO.ToLower().Contains(term)) ||
                            (i.LeadLegalInfos != null && i.LeadLegalInfos.LegalAddress != null && i.LeadLegalInfos.LegalAddress.ToLower().Contains(term)) ||
                            (i.LeadLegalInfos != null && i.LeadLegalInfos.FacticalAddress != null && i.LeadLegalInfos.FacticalAddress.ToLower().Contains(term)));
            }

            // Сортируем
            items = items.OrderBy(i => i.DateCreated);

            model.Fetched = items.ToList();
            return View(model);
        }

        /// <summary>
        /// Форма редактирования сведений по указанному лиду
        /// </summary>
        /// <param name="id">Идентификатор лида</param>
        /// <returns></returns>
        [Route("leads/{id}/edit")][AuthorizationCheck()]
        public ActionResult Edit(long id)
        {
            var lead = DataContext.Leads.FirstOrDefault(l => l.Id == id);
            if (lead == null)
            {
                ShowError("Не найден такой лид");
                return RedirectToAction("Index");
            }

            PushNavigationItem("Лиды", "/leads");
            PushNavigationItem("Редактирование лида "+lead.ToString(), "#");

            return View(lead);
        }

        /// <summary>
        /// Выполняет сохранение информации по лиду
        /// </summary>
        /// <param name="model">Модель с данными</param>
        /// <returns></returns>
        [Route("leads/save")][AuthorizationCheck]
        public ActionResult Save(Lead model)
        {
            var lead = DataContext.Leads.FirstOrDefault(l => l.Id == model.Id);
            if (lead == null)
            {
                ShowError("Такой лид не найден");
                return RedirectToAction("Index");
            }

            // Основные данные
            lead.FirstName = model.FirstName;
            lead.SurName = model.SurName;
            lead.LastName = model.LastName;
            lead.Phone = model.Phone;
            lead.Email = model.Email;

            // Паспортные
            if (lead.LeadPassportInfos == null)
            {
                lead.LeadPassportInfos = new LeadPassportInfo()
                {
                    Lead = lead
                };
            }
            lead.LeadPassportInfos.Number = model.LeadPassportInfos.Number;
            lead.LeadPassportInfos.Series = model.LeadPassportInfos.Series;
            lead.LeadPassportInfos.IssuedBy = model.LeadPassportInfos.IssuedBy;
            lead.LeadPassportInfos.IssueDate = model.LeadPassportInfos.IssueDate;

            // Юридические
            if (lead.LeadLegalInfos == null)
            {
                lead.LeadLegalInfos = new LeadLegalInfo()
                {
                    Lead = lead
                };
            }
            lead.LeadLegalInfos.LegalType = model.LeadLegalInfos.LegalType;
            lead.LeadLegalInfos.CompanyName = model.LeadLegalInfos.CompanyName;
            lead.LeadLegalInfos.DirectorFIO = model.LeadLegalInfos.DirectorFIO;
            lead.LeadLegalInfos.INN = model.LeadLegalInfos.INN;
            lead.LeadLegalInfos.OGRN = model.LeadLegalInfos.OGRN;
            lead.LeadLegalInfos.KPP = model.LeadLegalInfos.KPP;
            lead.LeadLegalInfos.OKPO = model.LeadLegalInfos.OKPO;
            lead.LeadLegalInfos.LegalAddress = model.LeadLegalInfos.LegalAddress;
            lead.LeadLegalInfos.FacticalAddress = model.LeadLegalInfos.FacticalAddress;

            // Рассчетные данные
            if (lead.LeadAccountInfos == null)
            {
                lead.LeadAccountInfos = new LeadAccountInfo()
                {
                    Lead = lead
                };
            }
            lead.LeadAccountInfos.Number = model.LeadAccountInfos.Number;
            lead.LeadAccountInfos.BIK = model.LeadAccountInfos.BIK;
            lead.LeadAccountInfos.BankName = model.LeadAccountInfos.BankName;
            lead.LeadAccountInfos.KNumber = model.LeadAccountInfos.KNumber;

            // TODO: добавить историю редактирования лидов

            DataContext.SubmitChanges();

            ShowSuccess("Данные были успешно сохранены");

            return RedirectToAction("Edit", new {id = lead.Id});
        }

        /// <summary>
        /// Удаляет указанного лида, если в лиде нет заказов
        /// </summary>
        /// <param name="id">Идентификатор лида</param>
        /// <returns></returns>
        [Route("leads/{id}/delete")]
        public ActionResult Delete(long id)
        {
            var lead = DataContext.Leads.FirstOrDefault(l => l.Id == id);
            if (lead == null)
            {
                ShowError("Такой лид не найден");
                return RedirectToAction("Index");
            }

            if (lead.LeadOrders.Count > 0)
            {
                ShowError("Нельзя удалить лида, у которого есть заказы");
                return RedirectToAction("Index");
            }

            DataContext.Leads.DeleteOnSubmit(lead);
            DataContext.SubmitChanges();
            ShowSuccess("Лид был успешно удален");

            return RedirectToAction("Index");

        }

    }
}

