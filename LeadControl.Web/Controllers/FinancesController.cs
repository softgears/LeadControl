using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Enums;
using LeadControl.Domain.Routing;
using LeadControl.Web.Classes.Security;
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
        [Route("finances")][AuthorizationCheck(Permission.Finances)]
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

            PushNavigationItem("Финансы", "/orders");
            PushNavigationItem("Оплаты по заказам", "#");

            model.Fetched = orders.OrderByDescending(d => d.DateModified).ToList();

            return View(model);
        }

    }
}
