﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Enums;
using LeadControl.Domain.Routing;
using LeadControl.Web.Classes.Security;
using LeadControl.Web.Models.Agreements;
using LeadControl.Web.Models.Orders;

namespace LeadControl.Web.Controllers
{
    /// <summary>
    /// Контроллер управления бухгатерской информацией по заказам
    /// </summary>
    public class FinancesController : BaseController
    {
        /// <summary>
        /// Отображает список заказов, над которыми доступны финансовые операции
        /// </summary>
        /// <param name="model">Модель данных для фильтрации</param>
        /// <returns></returns>
        [Route("finances/orders")][AuthorizationCheck(Permission.Finances)]
        public ActionResult Index(OrdersFiltrationModel model)
        {
            // Выбираем
            var projects = CurrentUser.IsAdmin() ? DataContext.Projects.Select(p => p.Id) : CurrentUser.ProjectUsers.Select(p => p.ProjectId);
            IEnumerable<LeadOrder> orders = DataContext.LeadOrders.Where(o => projects.Contains(o.ProjectId) && o.Status > (short)LeadOrderStatus.Initial && o.Status != (short)LeadOrderStatus.Completed);
            if (model.LeadIds.Length > 0)
            {
                orders = orders.Where(o => model.LeadIds.Contains(o.LeadId));
            }
            if (model.ProjectIds.Length > 0)
            {
                orders = orders.Where(o => model.ProjectIds.Contains(o.ProjectId));
            }
            if (model.Statuses.Length > 0)
            {
                orders = orders.Where(o => model.Statuses.Contains(o.Status));
            }
            if (model.DeliveryTypes.Length > 0)
            {
                orders = orders.Where(o => model.DeliveryTypes.Contains(o.DeliveryType));
            }
            if (model.PaymentTypes.Length > 0)
            {
                orders = orders.Where(o => model.PaymentTypes.Contains(o.PaymentType));
            }
            if (model.UsersIds.Length > 0)
            {
                orders = orders.Where(o => model.UsersIds.Contains(o.AssignedUserId) || o.LeadOrderChangements.Any(ac => model.UsersIds.Contains(ac.NewAssignedUserId) || model.UsersIds.Contains(ac.OldAssignedUserId)));
            }
            if (model.ProductTypesIds.Length > 0)
            {
                orders = orders.Where(o => o.LeadOrderItems.Any(oi => model.ProductTypesIds.Contains(oi.ProductId)));
            }
            if (model.Payed)
            {
                orders =
                    orders.Where(
                        o =>
                            o.LeadOrderPayments.Sum(p => p.Amount) >=
                            o.LeadOrderItems.Sum(loi => loi.Price * loi.Quantity));
            }
            if (!String.IsNullOrEmpty(model.Term))
            {
                var term = model.Term.ToLower();
                orders =
                    orders.Where(
                        o =>
                            (o.DeliveryAddress != null && o.DeliveryAddress.ToLower().Contains(term) ||
                             (o.LeadOrdersComments.Any(loc => loc.Comments != null && loc.Comments.Contains(term))) ||
                             (o.LeadOrderChangements.Any(loc => loc.Comments != null && loc.Comments.Contains(term)))));
            }

            PushNavigationItem("Бухгалтерия", "/logistics");
            PushNavigationItem("Оплаты по заказам", "#");

            model.Fetched = orders.OrderByDescending(d => d.DateModified).ToList();

            return View(model);
        }


        /// <summary>
        /// Отображает карточку указанного заказа
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <returns></returns>
        [Route("finances/{id}/info")]
        [AuthorizationCheck()]
        public ActionResult Info(long id)
        {
            var order = DataContext.LeadOrders.FirstOrDefault(lo => lo.Id == id);
            if (order == null)
            {
                ShowError("Такой заказ не найден");
                return RedirectToAction("Index");
            }

            PushNavigationItem("Бухгалтерия", "/finances");
            PushNavigationItem(string.Format("Информация о заказе №{0} для {1}", order.Id, order.Lead.ToString()), "#");

            return View("LeadOrderInfo", order);
        }

        /// <summary>
        /// Обрабатывает добавление платежа за заказ
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <param name="amount">Сумма оплаты</param>
        /// <param name="paymentType">Тип оплаты</param>
        /// <param name="customer">Информация о заказчике</param>
        /// <param name="document">Информация о платежке</param>
        /// <returns></returns>
        [HttpPost][AuthorizationCheck(Permission.Finances)][Route("finances/add-order-payment")]
        public ActionResult AddOrderPayment(long id, decimal amount, short paymentType, string customer, string document)
        {
            var order = DataContext.LeadOrders.FirstOrDefault(lo => lo.Id == id);
            if (order == null)
            {
                ShowError("Такой заказ не найден");
                return RedirectToAction("Index");
            }

            order.LeadOrderPayments.Add(new LeadOrderPayment()
            {
                LeadOrder = order,
                Amount = amount,
                User = CurrentUser,
                Customer = customer,
                PaymentType = paymentType,
                DocumentNumber = document,
                DateCreated = DateTime.Now
            });
            order.LeadOrderChangements.Add(new LeadOrderChangement()
            {
                Author = CurrentUser,
                DateCreated = DateTime.Now,
                NewStatus = order.Status,
                OldStatus = order.Status,
                NewWarehouseId = order.AssignedWarehouseId,
                OldWarehouseId = order.AssignedWarehouseId,
                OldAssignedUserId = order.AssignedUserId,
                NewAssignedUserId = order.AssignedUserId,
                Comments = String.Format("Поступление оплаты за заказу в размере {0:c} по документу {1} от {2}", amount, document, customer)
            });
            order.DateModified = DateTime.Now;
            DataContext.SubmitChanges();

            return Redirect(string.Format("/finances/{0}/info#money", id));
        }

        /// <summary>
        /// Отображает список договоров исходя из указанных условий фильтрации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("finances/agreements")][AuthorizationCheck(Permission.Finances)]
        public ActionResult AgreementsIndex(AgreementsFiltrationModel model)
        {
            // Выбираем
            var projects = CurrentUser.IsAdmin() ? DataContext.Projects.Select(p => p.Id) : CurrentUser.ProjectUsers.Select(p => p.ProjectId);
            IEnumerable<LeadAgreement> agreements = DataContext.LeadAgreements.Where(o => o.Status >= (short) LeadAgreementStatus.Signed && projects.Contains(o.ProjectId));
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

            PushNavigationItem("Бухгалтерия", "/finances/agreements");
            PushNavigationItem("Оплата по договорам", "#");

            return View(model);
        }

        /// <summary>
        /// Отображает карточку указанного договора
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <returns></returns>
        [Route("finances/agreements/{id}/info")]
        [AuthorizationCheck()]
        public ActionResult AgreementInfo(long id)
        {
            var agreement = DataContext.LeadAgreements.FirstOrDefault(lo => lo.Id == id);
            if (agreement == null)
            {
                ShowError("Такой договор не найден");
                return RedirectToAction("AgreementsIndex");
            }

            PushNavigationItem("Бухгалтерия", "/finances/agreements");
            PushNavigationItem(string.Format("Информация о договоре №{0} для {1}", agreement.Number, agreement.Lead.ToString()), "#");

            return View("LeadAgreementInfo", agreement);
        }

        /// <summary>
        /// Обрабатывает добавление платежа за договору
        /// </summary>
        /// <param name="id">Идентификатор договора</param>
        /// <param name="amount">Сумма оплаты</param>
        /// <param name="paymentType">Тип оплаты</param>
        /// <param name="customer">Информация о заказчике</param>
        /// <param name="document">Информация о платежке</param>
        /// <returns></returns>
        [HttpPost][AuthorizationCheck(Permission.Finances)][Route("finances/add-agreement-payment")]
        public ActionResult AddAgreementPayment(long id, decimal amount, short paymentType, string customer, string document)
        {
            var agreement = DataContext.LeadAgreements.FirstOrDefault(lo => lo.Id == id);
            if (agreement == null)
            {
                ShowError("Такой договор не найден");
                return RedirectToAction("AgreementsIndex");
            }

            agreement.LeadAgreementPayments.Add(new LeadAgreementPayment()
            {
                LeadAgreement = agreement,
                Amount = amount,
                User = CurrentUser,
                Customer = customer,
                PaymentType = paymentType,
                DocumentNumber = document,
                DateCreated = DateTime.Now
            });
            agreement.LeadAgreementChangements.Add(new LeadAgreementChangement()
            {
                User = CurrentUser,
                DateCreated = DateTime.Now,
                Comments = String.Format("Поступление оплаты за договору в размере {0:c} по документу {1} от {2}", amount, document, customer)
            });
            agreement.DateModified = DateTime.Now;
            DataContext.SubmitChanges();

            return Redirect(string.Format("/finances/agreements/{0}/info#money", id));
        }
    }
}
