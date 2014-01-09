using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Enums;
using LeadControl.Domain.Routing;
using LeadControl.Web.Classes.Security;
using LeadControl.Web.Models.Manage;

namespace LeadControl.Web.Controllers
{
    /// <summary>
    /// Контроллер управления типами услуг
    /// </summary>
    public class ManageServicesController : BaseController
    {
        /// <summary>
        /// Отображает главную страницу панели управления типами услуг
        /// </summary>
        /// <returns></returns>
        [Route("manage/services")][AuthorizationCheck(Permission.ManageServices)]
        public ActionResult Index(ServicesFiltrationModel model)
        {
            // Фильтруем
            var availableProjects = CurrentUser.ProjectUsers.Select(p => p.ProjectId).ToArray();
            IEnumerable<ServiceType> services = DataContext.ServiceTypes.Where(w => availableProjects.Contains(w.ProjectId));

            if (model.ProjectIds.Length > 0)
            {
                services = services.Where(p => model.ProjectIds.Contains(p.ProjectId));
            }
            if (model.ServiceTypes.Length > 0)
            {
                services = services.Where(p => model.ServiceTypes.Contains(p.Type));
            }
            if (model.ServicePeriods.Length > 0)
            {
                services = services.Where(p => model.ServicePeriods.Contains(p.PeriodType));
            }
            if (model.PaymentTypes.Length > 0)
            {
                services = services.Where(p => model.PaymentTypes.Contains(p.PaymentType));
            }
            if (!String.IsNullOrEmpty(model.Term))
            {
                var term = model.Term.ToLower();
                services =
                    services.Where(
                        p =>
                            p.Title.ToLower().Contains(term) ||
                            p.Description.ToLower().Contains(term));
            }

            model.Fetched = services.OrderByDescending(d => d.DateModified).ToList();

            PushNavigationItem("Управление услугами", "/manage/services");
            PushNavigationItem("Список услуг", "#");

            return View(model);
        }

        /// <summary>
        /// Отображает страницу добавления нового типа услуг
        /// </summary>
        /// <returns></returns>
        [Route("manage/services/add")][AuthorizationCheck(Permission.ManageServices)]
        public ActionResult Add()
        {
            PushNavigationItem("Управление услугами", "/manage/products");
            PushNavigationItem("Новый тип услуги", "#");

            return View("ServicesEditForm", new ServiceType()
            {
                PeriodType = (short)LeadServicePeriods.Montly,
                Type = (short) LeadServiceTypes.Periodical
            });
        }

        /// <summary>
        /// Отображает форму редактирования указанной услуги
        /// </summary>
        /// <param name="id">Идентификатор услуги</param>
        /// <returns></returns>
        [Route("manage/services/{id}/edit")]
        [AuthorizationCheck(Permission.ManageServices)]
        public ActionResult Edit(long id)
        {
            var serviceType = DataContext.ServiceTypes.FirstOrDefault(p => p.Id == id);
            if (serviceType == null)
            {
                ShowError("Такой тип услуги не найден");
                return RedirectToAction("Index");
            }

            PushNavigationItem("Управление услугами", "/manage/products");
            PushNavigationItem("Редактирование услуги " + serviceType.Title, "#");

            return View("ServicesEditForm", serviceType);
        }

        /// <summary>
        /// Удаляет указанный тип услуги, если с ним не было создано каких либо договоров
        /// </summary>
        /// <param name="id">Идентификатор типа услуги</param>
        /// <returns></returns>
        [Route("manage/services/{id}/delete")]
        [AuthorizationCheck(Permission.ManageProducts)]
        public ActionResult Delete(long id)
        {
            var serviceType = DataContext.ServiceTypes.FirstOrDefault(p => p.Id == id);
            if (serviceType == null)
            {
                ShowError("Такой тип услуги не найден");
                return RedirectToAction("Index");
            }

            if (serviceType.LeadAgreementServices.Count > 0)
            {
                ShowError("Нельзя удалить этот тип услуги, потому что в системе есть договора, его используюшие");
                return RedirectToAction("Index");
            }


            DataContext.ServiceTypes.DeleteOnSubmit(serviceType);
            DataContext.SubmitChanges();
            ShowSuccess("Тип услуги был успешно удален");

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Обрабатывает сохранение
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("manage/services/save")]
        [AuthorizationCheck(Permission.ManageServices)]
        public ActionResult Save(ServiceType model)
        {
            if (model.Id <= 0)
            {
                model.DateCreated = DateTime.Now;
                DataContext.ServiceTypes.InsertOnSubmit(model);
                DataContext.SubmitChanges();

                ShowSuccess("Тип услуги был успешно создан");
            }
            else
            {
                var service = DataContext.ServiceTypes.FirstOrDefault(p => p.Id == model.Id);
                if (service == null)
                {
                    ShowSuccess("Такой тип услуги не найден");
                    return RedirectToAction("Index");
                }

                service.Title = model.Title;
                service.Description = model.Description;
                service.Price = model.Price;
                service.Type = model.Type;
                service.PeriodType = model.PeriodType;
                service.PaymentType = model.PaymentType;
                service.DateModified = DateTime.Now;

                DataContext.SubmitChanges();

                ShowSuccess(string.Format("Услуга {0} была успешно отредактирована", service.Title));
            }

            return RedirectToAction("Index");
        }
    }
}
