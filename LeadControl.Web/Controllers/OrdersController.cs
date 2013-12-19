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
    /// Контроллер управления заявками
    /// </summary>
    public class OrdersController : BaseController
    {
        /// <summary>
        /// Отображает список заказов исходя из указанных условий фильтрации
        /// </summary>
        /// <returns></returns>
        [Route("orders")][AuthorizationCheck(Permission.Manager)]
        public ActionResult Index(OrdersFiltrationModel model)
        {
            // Выбираем
            var projects = CurrentUser.IsAdmin() ? DataContext.Projects.Select(p => p.Id) : CurrentUser.ProjectUsers.Select(p => p.ProjectId);
            IEnumerable<LeadOrder> orders = DataContext.LeadOrders.Where(o => projects.Contains(o.ProjectId));
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
                orders = orders.Where(o => model.UsersIds.Contains(o.AssignedUserId) || o.LeadOrderAssignedUserChangements.Any(ac => model.UsersIds.Contains(ac.NewAssignedUserId)));
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
                            o.LeadOrderItems.Sum(loi => loi.Price*loi.Quantity));
            }
            if (!String.IsNullOrEmpty(model.Term))
            {
                var term = model.Term.ToLower();
                orders =
                    orders.Where(
                        o =>
                            (o.DeliveryAddress != null && o.DeliveryAddress.ToLower().Contains(term) ||
                             (o.LeadOrdersComments.Any(loc => loc.Comments != null && loc.Comments.Contains(term))) ||
                             (o.LeadOrderStatusChangements.Any(loc => loc.Comments != null && loc.Comments.Contains(term)))));
            }
            
            PushNavigationItem("Заказы","/orders");
            PushNavigationItem("Список заказов","#");

            model.Fetched = orders.OrderByDescending(d => d.DateModified).ToList();

            return View(model);
        }

        /// <summary>
        /// Отображает карточку указанного заказа
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <returns></returns>
        [Route("orders/{id}/info")][AuthorizationCheck()]
        public ActionResult Info(long id)
        {
            var order = DataContext.LeadOrders.FirstOrDefault(lo => lo.Id == id);
            if (order == null)
            {
                ShowError("Такой заказ не найден");
                return RedirectToAction("Index");
            }

            PushNavigationItem("Заказы", "/orders");
            PushNavigationItem(string.Format("Информация о заказе №{0} для {1}", order.Id, order.Lead.ToString()), "#");

            return View("LeadOrderInfo", order);
        }

        /// <summary>
        /// Отображает форму создания нового заказа
        /// </summary>
        /// <returns></returns>
        [Route("orders/create")][AuthorizationCheck(Permission.Manager)]
        public ActionResult Create()
        {
            PushNavigationItem("Заказы", "/orders");
            PushNavigationItem("Создание нового заказа. Шаг 1", "#");

            return View();
        }

        /// <summary>
        /// Обрабатывает создание заявки на первом шаге
        /// </summary>
        /// <param name="LeadId">Идентификатор лида сделавшего заказ</param>
        /// <param name="ProjectId">Идентификатор проекта в рамках которого создается заказ</param>
        /// <param name="model">Модель данных по новому лиду</param>
        /// <returns></returns>
        [Route("orders/create-1")][AuthorizationCheck(Permission.Manager)][HttpPost]
        public ActionResult Create1Step(long LeadId, long ProjectId, Lead model)
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

            // создаем заказ
            var order = new LeadOrder()
            {
                DateCreated = DateTime.Now,
                User = CurrentUser,
                DeliveryType = (short) DeliveryTypes.Self,
                PaymentType = (short) PaymentTypes.BankPayment,
                Status = (short) LeadOrderStatus.Initial,
                ProjectId = ProjectId
            };

            // Создаем первоначальные данные по истории заявки
            order.LeadOrderStatusChangements.Add(new LeadOrderStatusChangement()
            {
                AuthorId = CurrentUser.Id,
                LeadOrder = order,
                DateCreated = DateTime.Now,
                Status = (short)LeadOrderStatus.Initial,
                Comments = "Создание заказа пользователем "+CurrentUser.GetFio()
            });

            lead.LeadOrders.Add(order);
            DataContext.SubmitChanges();

            ShowSuccess(string.Format("Заказ №{0} успешно создан для лида {1}", order.Id, lead.ToString()));

            return RedirectToAction("EditOrderItems", new {id = order.Id});
        }

        /// <summary>
        /// Отображает второй этап создания заказа - заполнение позиций в заказе
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <returns></returns>
        [Route("orders/create-edit-items/{id}")][AuthorizationCheck(Permission.Manager)]
        public ActionResult EditOrderItems(long id)
        {
            var order = DataContext.LeadOrders.FirstOrDefault(lo => lo.Id == id);
            if (order == null)
            {
                ShowError("Такой заказ не найден");
                return RedirectToAction("Index");
            }

            // Проверяем что мы можем его редактировать
            if (order.Status != (short)LeadOrderStatus.Initial)
            {
                ShowError("Можно редактировать только те заказы, которые находятся в статусе формирования");
                return RedirectToAction("Info",new {id});
            }

            PushNavigationItem("Заказы", "/orders");
            PushNavigationItem("Создание нового заказа. Шаг 2 - Заполнение позиций", "#");

            return View(order);
        }
    }
}
