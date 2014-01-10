using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Enums;
using LeadControl.Domain.Routing;
using LeadControl.Web.Classes.Ext;
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

            return RedirectToAction("Info", new {id = agreement.Id});
        }

        /// <summary>
        /// Отображает карточку указанного договора
        /// </summary>
        /// <param name="id">Идентификатор договора</param>
        /// <returns></returns>
        [Route("agreements/{id}/info")][AuthorizationCheck()]
        public ActionResult Info(long id)
        {
            var projects = CurrentUser.IsAdmin() ? DataContext.Projects.Select(p => p.Id) : CurrentUser.ProjectUsers.Select(p => p.ProjectId);
            var agreement = DataContext.LeadAgreements.FirstOrDefault(a => a.Id == id && projects.Contains(a.ProjectId));
            if (agreement == null)
            {
                ShowError("Такой договор не найден");
                return RedirectToAction("Index");
            }

            PushNavigationItem("Договора", "/agreements");
            PushNavigationItem("Договор №"+agreement.Number, "#");

            return View(agreement);
        }

        /// <summary>
        /// Обрабатывает изменение типа услугии у позиции в договоре
        /// </summary>
        /// <param name="id">Идентификатор договора</param>
        /// <param name="itemId">Идентификатор позиции</param>
        /// <param name="serviceId">Идентификатор типа услуги</param>
        /// <param name="newId">Новый идентификатор типа услуги</param>
        /// <returns></returns>
        [HttpPost]
        [Route("agreements/change-agreement-item-service")]
        [AuthorizationCheck(Permission.ServicesManager)]
        public ActionResult ChangeAgreementItemService(long id, long itemId, long serviceId, long newId)
        {
            var agreement = DataContext.LeadAgreements.FirstOrDefault(o => o.Id == id);
            if (agreement == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Договор не найден"
                });
            }

            if (agreement.Status != (short)LeadAgreementStatus.Negotiation)
            {
                return Json(new
                {
                    success = false,
                    msg = "Нельзя редактировать позиции договора в этом статусе"
                });
            }

            // Ищем позицию
            var serviceType = agreement.Project.ServiceTypes.First(s => s.Id == newId);
            var totalPeriods = serviceType.ComputeTotalPeriods(agreement);
            LeadAgreementService agreementService;
            if (serviceId == 0 && itemId == 0)
            {
                agreementService = new LeadAgreementService()
                {
                    LeadAgreement = agreement,
                    Price = serviceType.Price,
                    Period = totalPeriods,
                    ServiceType = serviceType
                };
                agreement.LeadAgreementServices.Add(agreementService);
            }
            else
            {
                agreementService = agreement.LeadAgreementServices.FirstOrDefault(oi => oi.Id == itemId);
                if (agreementService == null)
                {
                    return Json(new
                    {
                        success = false,
                        msg = "Такая позиция не найдена"
                    });
                }

                // Изменяем продукт
                if (serviceType == null)
                {
                    return Json(new
                    {
                        success = false,
                        msg = "Новый тип услуги не найден"
                    });
                }
                agreementService.ServiceType.LeadAgreementServices.Remove(agreementService);
                serviceType.LeadAgreementServices.Add(agreementService);
                agreementService.DateModified = DateTime.Now;
            }

            // Пытаемся сохранить
            try
            {
                DataContext.SubmitChanges();
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    msg = e.Message
                });
            }

            // Отдаем успешный результат
            return Json(new
            {
                success = true,
                id = agreementService.Id,
                serviceId = agreementService.ServiceTypeId,
                price = agreementService.Price.ToString("0.00"),
                period = ((LeadServicePeriods)agreementService.ServiceType.PeriodType).GetEnumMemberName(),
                periods = totalPeriods
            });
        }

        /// <summary>
        /// Обрабатывает изменение общего количества оказаний услуги в договоре
        /// </summary>
        /// <param name="id">Идентификатор договора</param>
        /// <param name="itemId">Идентификатор позиции</param>
        /// <param name="serviceId">Идентификатор типа услуги</param>
        /// <param name="quantity">Новое количество</param>
        /// <returns></returns>
        [HttpPost]
        [Route("agreements/change-agreement-item-period")]
        [AuthorizationCheck(Permission.ServicesManager)]
        public ActionResult ChangeAgreementItemPeriods(long id, long itemId, long serviceId, int quantity)
        {
            var agreement = DataContext.LeadAgreements.FirstOrDefault(o => o.Id == id);
            if (agreement == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Договор не найден"
                });
            }

            if (agreement.Status != (short)LeadAgreementStatus.Negotiation)
            {
                return Json(new
                {
                    success = false,
                    msg = "Нельзя редактировать позиции договора в этом статусе"
                });
            }

            // Ищем позицию
            var serviceType = serviceId == 0 ? agreement.Project.ServiceTypes.First() : agreement.Project.ServiceTypes.First(s => s.Id == serviceId);
            LeadAgreementService agreementService;
            if (serviceId == 0 && itemId == 0)
            {
                agreementService = new LeadAgreementService()
                {
                    LeadAgreement = agreement,
                    Price = serviceType.Price,
                    Period = quantity,
                    ServiceType = serviceType
                };
                agreement.LeadAgreementServices.Add(agreementService);
            }
            else
            {
                agreementService = agreement.LeadAgreementServices.FirstOrDefault(oi => oi.Id == itemId);
                if (agreementService == null)
                {
                    return Json(new
                    {
                        success = false,
                        msg = "Такая позиция не найдена"
                    });
                }

                // Изменяем продукт
                if (serviceType == null)
                {
                    return Json(new
                    {
                        success = false,
                        msg = "Новый тип услуги не найден"
                    });
                }
                agreementService.Period = quantity;
                agreementService.DateModified = DateTime.Now;
            }

            // Пытаемся сохранить
            try
            {
                DataContext.SubmitChanges();
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    msg = e.Message
                });
            }

            // Отдаем успешный результат
            return Json(new
            {
                success = true,
                id = agreementService.Id,
                serviceId = agreementService.ServiceTypeId,
            });
        }

        /// <summary>
        /// Обрабатывает изменение стоимости оказания услуги у договора
        /// </summary>
        /// <param name="id">Идентификатор договора</param>
        /// <param name="itemId">Идентификатор позиции</param>
        /// <param name="serviceId">Идентификатор типа услуги</param>
        /// <param name="price">Новая цена</param>
        /// <returns></returns>
        [HttpPost]
        [Route("agreements/change-agreement-item-price")]
        [AuthorizationCheck(Permission.ServicesManager)]
        public ActionResult ChangeAgreementItemPrice(long id, long itemId, long serviceId, decimal price)
        {
            var agreement = DataContext.LeadAgreements.FirstOrDefault(o => o.Id == id);
            if (agreement == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Договор не найден"
                });
            }

            if (agreement.Status != (short)LeadAgreementStatus.Negotiation)
            {
                return Json(new
                {
                    success = false,
                    msg = "Нельзя редактировать позиции договора в этом статусе"
                });
            }

            // Ищем позицию
            var serviceType = serviceId == 0 ? agreement.Project.ServiceTypes.First() : agreement.Project.ServiceTypes.First(s => s.Id == serviceId);
            LeadAgreementService agreementService;
            if (serviceId == 0 && itemId == 0)
            {
                agreementService = new LeadAgreementService()
                {
                    LeadAgreement = agreement,
                    Price = serviceType.Price,
                    Period = 1,
                    ServiceType = serviceType
                };
                agreement.LeadAgreementServices.Add(agreementService);
            }
            else
            {
                agreementService = agreement.LeadAgreementServices.FirstOrDefault(oi => oi.Id == itemId);
                if (agreementService == null)
                {
                    return Json(new
                    {
                        success = false,
                        msg = "Такая позиция не найдена"
                    });
                }

                // Изменяем продукт
                if (serviceType == null)
                {
                    return Json(new
                    {
                        success = false,
                        msg = "Новый тип услуги не найден"
                    });
                }
                agreementService.Price = price;
                agreementService.DateModified = DateTime.Now;
            }

            // Пытаемся сохранить
            try
            {
                DataContext.SubmitChanges();
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    msg = e.Message
                });
            }

            // Отдаем успешный результат
            return Json(new
            {
                success = true,
                id = agreementService.Id,
                serviceId = agreementService.ServiceTypeId,
            });
        }

        /// <summary>
        /// Обрабатывает удаление указанной позиции в договоре
        /// </summary>
        /// <param name="id">Идентификатор договора</param>
        /// <param name="itemId">Идентификатор позиции</param>
        /// <returns></returns>
        [HttpPost]
        [Route("agreements/delete-agreement-item")]
        [AuthorizationCheck(Permission.ServicesManager)]
        public ActionResult DeleteAgreementItem(long id, long itemId)
        {
            // Ищем заказ
            var agreement = DataContext.LeadAgreements.FirstOrDefault(o => o.Id == id);
            if (agreement == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Договор не найден"
                });
            }

            if (agreement.Status != (short)LeadAgreementStatus.Negotiation)
            {
                return Json(new
                {
                    success = false,
                    msg = "Нельзя редактировать позиции договора в этом статусе"
                });
            }

            // Ищем позицию
            var agreementItem = agreement.LeadAgreementServices.FirstOrDefault(oi => oi.Id == itemId);
            if (agreementItem == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Такая позиция не найдена"
                });
            }

            // Удаляем
            DataContext.LeadAgreementServices.DeleteOnSubmit(agreementItem);

            // Пытаемся сохранить
            try
            {
                DataContext.SubmitChanges();
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    msg = e.Message
                });
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// Обрабатывает изменение данных договора
        /// </summary>
        /// <param name="model">Модель измененных данных</param>
        /// <param name="comments">Комментарии к изменению</param>
        /// <returns></returns>
        [HttpPost][Route("agreements/update-agreement-info")][AuthorizationCheck(Permission.ServicesManager)]
        public ActionResult UpdateAgreementInfo(LeadAgreement model, string comments)
        {
            var projects = CurrentUser.IsAdmin() ? DataContext.Projects.Select(p => p.Id) : CurrentUser.ProjectUsers.Select(p => p.ProjectId);
            var agreement = DataContext.LeadAgreements.FirstOrDefault(a => a.Id == model.Id && projects.Contains(a.ProjectId));
            if (agreement == null)
            {
                ShowError("Такой договор не найден");
                return RedirectToAction("Index");
            }

            // Валидируем номер договора - он должен быть уникальным
            var existed =
                agreement.Project.LeadAgreements.Any(
                    a => a.Number.ToLower() == model.Number.ToLower() && a.Id != agreement.Id);
            if (existed)
            {
                ShowError("Договор с таким номером уже есть в системе");
                return RedirectToAction("Info", new {id = agreement.Id});
            }

            // Изменение
            var changement = new LeadAgreementChangement()
            {
                Comments = comments,
                User = CurrentUser,
                LeadAgreement = agreement,
                DateCreated = DateTime.Now
            };
            if (model.Number != agreement.Number)
            {
                changement.LeadAgreementChangementValues.Add(new LeadAgreementChangementValue()
                {
                    LeadAgreementChangement = changement,
                    OldValue = agreement.Number,
                    NewValue = model.Number,
                    PropertyName = "Номер"
                });
                agreement.Number = model.Number;
            }
            if (model.Date != agreement.Date)
            {
                changement.LeadAgreementChangementValues.Add(new LeadAgreementChangementValue()
                {
                    LeadAgreementChangement = changement,
                    OldValue = agreement.Date.FormatDate(),
                    NewValue = model.Date.FormatDate(),
                    PropertyName = "Дата заключения"
                });
                agreement.Date = model.Date;
            }
            if (model.EndDate != agreement.EndDate)
            {
                changement.LeadAgreementChangementValues.Add(new LeadAgreementChangementValue()
                {
                    LeadAgreementChangement = changement,
                    OldValue = agreement.EndDate.FormatDate(),
                    NewValue = model.EndDate.FormatDate(),
                    PropertyName = "Дата завершения действия"
                });
                agreement.EndDate = model.EndDate;
            }
            if (model.EndDate != agreement.EndDate)
            {
                changement.LeadAgreementChangementValues.Add(new LeadAgreementChangementValue()
                {
                    LeadAgreementChangement = changement,
                    OldValue = agreement.EndDate.FormatDate(),
                    NewValue = model.EndDate.FormatDate(),
                    PropertyName = "Дата завершения действия"
                });
                agreement.EndDate = model.EndDate;
            }
            if (model.Title != agreement.Title)
            {
                changement.LeadAgreementChangementValues.Add(new LeadAgreementChangementValue()
                {
                    LeadAgreementChangement = changement,
                    OldValue = agreement.Title,
                    NewValue = model.Title,
                    PropertyName = "Название"
                });
                agreement.Title = model.Title;
            }
            if (model.Description != agreement.Description)
            {
                changement.LeadAgreementChangementValues.Add(new LeadAgreementChangementValue()
                {
                    LeadAgreementChangement = changement,
                    OldValue = agreement.Description,
                    NewValue = model.Description,
                    PropertyName = "Описание"
                });
                agreement.Description = model.Description;
            }
            if (model.Status != agreement.Status)
            {
                changement.LeadAgreementChangementValues.Add(new LeadAgreementChangementValue()
                {
                    LeadAgreementChangement = changement,
                    OldValue = ((LeadAgreementStatus)agreement.Status).GetEnumMemberName(),
                    NewValue = ((LeadAgreementStatus)model.Status).GetEnumMemberName(),
                    PropertyName = "Статус"
                });
                agreement.Status = model.Status;
            }
            if (changement.LeadAgreementChangementValues.Count > 0)
            {
                agreement.LeadAgreementChangements.Add(changement);
                agreement.DateModified = DateTime.Now;
                DataContext.SubmitChanges();
            }

            ShowSuccess(string.Format("Договор №{0} был успешно сохранен", agreement.Number));

            return RedirectToAction("Info", new {id = agreement.Id});
        }

    }
}
