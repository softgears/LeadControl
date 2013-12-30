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
        [Route("orders")]
        [AuthorizationCheck(Permission.Manager)]
        public ActionResult Index(OrdersFiltrationModel model)
        {
            // Выбираем
            var projects = CurrentUser.IsAdmin() ? DataContext.Projects.Select(p => p.Id) : CurrentUser.ProjectUsers.Select(p => p.ProjectId);
            IEnumerable<LeadOrder> orders = DataContext.LeadOrders.Where(o => projects.Contains(o.ProjectId));
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

            PushNavigationItem("Заказы", "/orders");
            PushNavigationItem("Список заказов", "#");

            model.Fetched = orders.OrderByDescending(d => d.DateModified).ToList();

            return View(model);
        }

        /// <summary>
        /// Отображает карточку указанного заказа
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <returns></returns>
        [Route("orders/{id}/info")]
        [AuthorizationCheck()]
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
        [Route("orders/create")]
        [AuthorizationCheck(Permission.Manager)]
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
        [Route("orders/create-1")]
        [AuthorizationCheck(Permission.Manager)]
        [HttpPost]
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
                DeliveryType = (short)DeliveryTypes.Self,
                PaymentType = (short)PaymentTypes.BankPayment,
                Status = (short)LeadOrderStatus.Initial,
                ProjectId = ProjectId
            };

            // Создаем первоначальные данные по истории заявки
            order.LeadOrderChangements.Add(new LeadOrderChangement()
            {
                AuthorId = CurrentUser.Id,
                LeadOrder = order,
                DateCreated = DateTime.Now,
                NewStatus = (short)LeadOrderStatus.Initial,
                Comments = "Создание заказа пользователем " + CurrentUser.GetFio()
            });

            lead.LeadOrders.Add(order);
            DataContext.SubmitChanges();

            ShowSuccess(string.Format("Заказ №{0} успешно создан для лида {1}", order.Id, lead.ToString()));

            return RedirectToAction("EditOrderItems", new { id = order.Id });
        }

        /// <summary>
        /// Отображает второй этап создания заказа - заполнение позиций в заказе
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <returns></returns>
        [Route("orders/create-edit-items/{id}")]
        [AuthorizationCheck(Permission.Manager)]
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
                return RedirectToAction("Info", new { id });
            }

            PushNavigationItem("Заказы", "/orders");
            PushNavigationItem("Создание нового заказа. Шаг 2 - Заполнение позиций", "#");

            return View(order);
        }

        /// <summary>
        /// Обрабатывает изменение типа продукта у позиции в заявки
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <param name="itemId">Идентификатор позиции</param>
        /// <param name="productId">Идентификатор типа продукта</param>
        /// <param name="newId">Новый идентификатор продукта</param>
        /// <returns></returns>
        [HttpPost]
        [Route("orders/change-order-item-product")]
        [AuthorizationCheck(Permission.Manager)]
        public ActionResult ChangeOrderItemProduct(long id, long itemId, long productId, long newId)
        {
            var order = DataContext.LeadOrders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Заказ не найден"
                });
            }

            if (order.Status != (short)LeadOrderStatus.Initial)
            {
                return Json(new
                {
                    success = false,
                    msg = "Нельзя редактировать позиции заказа в этом статусе"
                });
            }

            // Ищем позицию
            LeadOrderItem orderItem;
            if (productId == 0 && itemId == 0)
            {
                orderItem = new LeadOrderItem()
                {
                    LeadOrder = order,
                    Price = 0,
                    Quantity = 0,
                    ProductId = newId
                };
                order.LeadOrderItems.Add(orderItem);
            }
            else
            {
                orderItem = order.LeadOrderItems.FirstOrDefault(oi => oi.Id == itemId);
                if (orderItem == null)
                {
                    return Json(new
                    {
                        success = false,
                        msg = "Такая позиция не найдена"
                    });
                }

                // Изменяем продукт
                var productType = order.Project.ProductTypes.FirstOrDefault(pt => pt.Id == newId);
                if (productType == null)
                {
                    return Json(new
                    {
                        success = false,
                        msg = "Новый тип продукта не найден"
                    });
                }
                orderItem.ProductType.LeadOrderItems.Remove(orderItem);
                productType.LeadOrderItems.Add(orderItem);
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
                id = orderItem.Id,
                productId = orderItem.ProductId
            });
        }

        /// <summary>
        /// Обрабатывает изменение количества у позиции в заказе
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <param name="itemId">Идентификатор позиции</param>
        /// <param name="productId">Идентификатор типа продукта</param>
        /// <param name="quantity">Новый идентификатор продукта</param>
        /// <returns></returns>
        [HttpPost]
        [Route("orders/change-order-item-quantity")]
        [AuthorizationCheck(Permission.Manager)]
        public ActionResult ChangeOrderItemQuantity(long id, long itemId, long productId, int quantity)
        {
            var order = DataContext.LeadOrders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Заказ не найден"
                });
            }

            if (order.Status != (short)LeadOrderStatus.Initial)
            {
                return Json(new
                {
                    success = false,
                    msg = "Нельзя редактировать позиции заказа в этом статусе"
                });
            }

            // Ищем позицию
            LeadOrderItem orderItem;
            if (productId == 0 && itemId == 0)
            {
                orderItem = new LeadOrderItem()
                {
                    LeadOrder = order,
                    Price = 0,
                    Quantity = quantity,
                    ProductId = order.Project.ProductTypes.First().Id
                };
                order.LeadOrderItems.Add(orderItem);
            }
            else
            {
                orderItem = order.LeadOrderItems.FirstOrDefault(oi => oi.Id == itemId);
                if (orderItem == null)
                {
                    return Json(new
                    {
                        success = false,
                        msg = "Такая позиция не найдена"
                    });
                }

                // Изменяем количество
                orderItem.Quantity = quantity;
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
                id = orderItem.Id,
                productId = orderItem.ProductId
            });
        }

        /// <summary>
        /// Обрабатывает изменение количества у позиции в заявки
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <param name="itemId">Идентификатор позиции</param>
        /// <param name="productId">Идентификатор типа продукта</param>
        /// <param name="price">Новая цена</param>
        /// <returns></returns>
        [HttpPost]
        [Route("orders/change-order-item-price")]
        [AuthorizationCheck(Permission.Manager)]
        public ActionResult ChangeOrderItemPrice(long id, long itemId, long productId, decimal price)
        {
            var order = DataContext.LeadOrders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Заказ не найден"
                });
            }

            if (order.Status != (short)LeadOrderStatus.Initial)
            {
                return Json(new
                {
                    success = false,
                    msg = "Нельзя редактировать позиции заказа в этом статусе"
                });
            }

            // Ищем позицию
            LeadOrderItem orderItem;
            if (productId == 0 && itemId == 0)
            {
                orderItem = new LeadOrderItem()
                {
                    LeadOrder = order,
                    Price = price,
                    Quantity = 0,
                    ProductId = order.Project.ProductTypes.First().Id
                };
                order.LeadOrderItems.Add(orderItem);
            }
            else
            {
                orderItem = order.LeadOrderItems.FirstOrDefault(oi => oi.Id == itemId);
                if (orderItem == null)
                {
                    return Json(new
                    {
                        success = false,
                        msg = "Такая позиция не найдена"
                    });
                }

                // Изменяем количество
                orderItem.Price = price;
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
                id = orderItem.Id,
                productId = orderItem.ProductId
            });
        }

        /// <summary>
        /// Обрабатывает удаление указанной позиции в заявке
        /// </summary>
        /// <param name="id">Идентификатор заявки</param>
        /// <param name="itemId">Идентификатор позиции</param>
        /// <returns></returns>
        [HttpPost]
        [Route("orders/delete-order-item")]
        [AuthorizationCheck(Permission.Manager)]
        public ActionResult DeleteOrderItem(long id, long itemId)
        {
            // Ищем заказ
            var order = DataContext.LeadOrders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Заказ не найден"
                });
            }

            if (order.Status != (short)LeadOrderStatus.Initial)
            {
                return Json(new
                {
                    success = false,
                    msg = "Нельзя редактировать позиции заказа в этом статусе"
                });
            }

            // Ищем позицию
            var orderItem = order.LeadOrderItems.FirstOrDefault(oi => oi.Id == itemId);
            if (orderItem == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Такая позиция не найдена"
                });
            }

            // Удаляем
            DataContext.LeadOrderItems.DeleteOnSubmit(orderItem);

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
        /// Отображает форму редактирования дополнительных сведений по заказу - способы доставки и оплаты
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <returns></returns>
        [Route("orders/create-info/{id}")]
        [AuthorizationCheck(Permission.Manager)]
        public ActionResult EditOrderInfo(long id)
        {
            var order = DataContext.LeadOrders.FirstOrDefault(lo => lo.Id == id);
            if (order == null)
            {
                ShowError("Такой заказ не найден");
                return RedirectToAction("Index");
            }

            PushNavigationItem("Заказы", "/orders");
            PushNavigationItem("Создание нового заказа. Шаг 3 - Информация по доставке и оплате", "#");

            return View(order);
        }

        /// <summary>
        /// Обрабатывает последний этап создания заявки
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <param name="paymentType">Способ оплаты</param>
        /// <param name="deliveryType">Способ доставки</param>
        /// <param name="deliveryAddress">Адрес доставки</param>
        /// <returns></returns>
        [Route("orders/create-3")]
        [HttpPost]
        [AuthorizationCheck(Permission.Manager)]
        public ActionResult Create3Step(long id, short paymentType, short deliveryType, string deliveryAddress)
        {
            var order = DataContext.LeadOrders.FirstOrDefault(lo => lo.Id == id);
            if (order == null)
            {
                ShowError("Такой заказ не найден");
                return RedirectToAction("Index");
            }

            if (order.Status == (short)LeadOrderStatus.Completed)
            {
                ShowError("Заказ уже выполнен");
                return RedirectToAction("Index");
            }

            order.DeliveryType = deliveryType;
            order.PaymentType = paymentType;
            order.DeliveryAddress = deliveryAddress;
            order.DateModified = DateTime.Now;
            DataContext.SubmitChanges();

            return RedirectToAction("Info", new { id });
        }

        /// <summary>
        /// Обрабатывает изменения статуса указанного заказа
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        /// <param name="newStatus">Новый статус</param>
        /// <param name="newUser">Новый пользователь</param>
        /// <param name="newWarehouse">Новый назначенный склад</param>
        /// <param name="comments">Комментарии</param>
        /// <param name="createFeaOrder">Создать ли заявку на дозакуп</param>
        /// <param name="redirectUrl">Url куда сделать редирект</param>
        /// <param name="extractFromWarehouse">Извлечь ли остатки товара с назначенного склада</param>
        /// <returns></returns>
        [HttpPost]
        [Route("orders/change-order-status")]
        [AuthorizationCheck()]
        public ActionResult ChangeOrderStatus(long id, short newStatus, long newUser, long newWarehouse, string comments,
            bool createFeaOrder, string redirectUrl, bool extractFromWarehouse = false)
        {
            var order = DataContext.LeadOrders.FirstOrDefault(lo => lo.Id == id);
            if (order == null)
            {
                ShowError("Такой заказ не найден");
                return RedirectToAction("Index");
            }

            if (!order.CanChangeStatus(CurrentUser))
            {
                ShowError("Такой заказ не найден");
                return RedirectToAction("Index");
            }

            // Изменяем статус
            var oldStatus = order.Status;
            var oldAssignedUserId = order.AssignedUserId;
            var oldWarehouse = order.AssignedWarehouseId;
            order.Status = newStatus;
            order.DateModified = DateTime.Now;
            if (newUser != oldAssignedUserId)
            {
                order.User.LeadOrders.Remove(order);
                DataContext.Users.First(u => u.Id == newUser).LeadOrders.Add(order);
            }
            if (newWarehouse != oldWarehouse)
            {
                if (order.Warehouse != null)
                {
                    order.Warehouse.LeadOrders.Remove(order);
                }
                DataContext.Warehouses.First(w => w.Id == newWarehouse).LeadOrders.Add(order);
            }
            order.LeadOrderChangements.Add(new LeadOrderChangement()
            {
                Author = CurrentUser,
                Comments = comments,
                OldStatus = oldStatus,
                OldAssignedUserId = oldAssignedUserId,
                OldWarehouseId = oldWarehouse,
                NewStatus = newStatus,
                NewAssignedUserId = newUser,
                NewWarehouseId = newWarehouse,
                LeadOrder = order,
                DateCreated = DateTime.Now
            });

            // Сохраняем
            DataContext.SubmitChanges();

            ShowSuccess(string.Format("Статус заказа №{0} для {1} был успешно изменен", order.Id, order.Lead.ToString()));

            if (extractFromWarehouse && order.Warehouse != null)
            {
                // Извлекаем остатки со склада
                foreach (var orderItem in order.LeadOrderItems)
                {
                    var warehouseProduct =
                        order.Warehouse.WarehouseProducts.FirstOrDefault(wp => wp.ProductId == orderItem.ProductId);
                    if (warehouseProduct == null)
                    {
                        warehouseProduct = new WarehouseProduct()
                        {
                            DateCreated = DateTime.Now,
                            ProductId = orderItem.ProductId,
                            Quantity = 0,
                            Warehouse = order.Warehouse,
                            Price = orderItem.Price
                        };
                        order.Warehouse.WarehouseProducts.Add(warehouseProduct);
                    }
                    warehouseProduct.Quantity = warehouseProduct.Quantity - orderItem.Quantity;
                    warehouseProduct.DateModified = DateTime.Now;
                    warehouseProduct.WarehouseProductChangements.Add(new WarehouseProductChangement()
                    {
                        DateCreated = DateTime.Now,
                        Amount = orderItem.Quantity,
                        Direction = (short)WarehouseProductChangementDirection.Outcome,
                        WarehouseProduct = warehouseProduct,
                        Description = String.Format("Изъятие остатков в количестве {0} для выполнения заказа №{1} пользователем {2}", orderItem.Quantity, order.Id, CurrentUser.GetFio())
                    });
                }
                order.LeadOrderChangements.Add(new LeadOrderChangement()
                {
                    Author = CurrentUser,
                    NewAssignedUserId = order.AssignedUserId,
                    OldAssignedUserId = order.AssignedUserId,
                    OldStatus = order.Status,
                    NewStatus = order.Status,
                    OldWarehouseId = order.AssignedWarehouseId,
                    NewWarehouseId = order.AssignedWarehouseId,
                    DateCreated = DateTime.Now,
                    Comments = String.Format("Извлечение остатков со склада {0} ({1}) в общем количестве {2}", order.Warehouse.Title, order.Warehouse.City, order.LeadOrderItems.Sum(oi => oi.Quantity))
                });
                DataContext.SubmitChanges();
                ShowSuccess(string.Format("Остатки со склада {0} ({1}) были успешно извлечены", order.Warehouse.Title, order.Warehouse.City));
            }

            // Создаем заявку на дозакуп
            if (createFeaOrder)
            {
                // Ищем менеджера
                var targetManagers =
                    order.Project.ProjectUsers.Where(u => u.User.HasPermission(Permission.FEA))
                        .Select(u => u.User)
                        .ToList();
                if (targetManagers.Count > 0)
                {
                    User targetManager = null;
                    targetManager = targetManagers.FirstOrDefault(u => u.RoleId != 1) ?? targetManagers.FirstOrDefault();
                    if (targetManager != null)
                    {
                        var newFeaOrder = new FEAOrder()
                        {
                            Description =
                                string.Format("Заявка на дозакуп, сформированная автоматически из заказа №{0}", order.Id),
                            DateCreated = DateTime.Now,
                            Status = (short)FEAOrderStatus.Gathering,
                            Manager = targetManager,
                            Project = order.Project,
                            TargetWarehouse = order.Warehouse
                        };
                        newFeaOrder.FEAOrderItems.AddRange(order.LeadOrderItems.Select(oi => new FEAOrderItem()
                        {
                            DateCreated = DateTime.Now,
                            FEAOrder = newFeaOrder,
                            Price = oi.Price,
                            ProductId = oi.ProductId,
                            Quantity = oi.Quantity*2
                        }));
                        newFeaOrder.FEAOrdersStatusChangements.Add(new FEAOrdersStatusChangement()
                        {
                            User = CurrentUser,
                            FEAOrder = newFeaOrder,
                            DateCreated = DateTime.Now,
                            Status = (short)FEAOrderStatus.Gathering,
                            Comments = string.Format("Автоматическое создание заявку на дозакуп из заказа №{0}", order.Id)
                        });
                        order.LeadOrderChangements.Add(new LeadOrderChangement()
                        {
                            Author = CurrentUser,
                            NewAssignedUserId = order.AssignedUserId,
                            OldAssignedUserId = order.AssignedUserId,
                            OldStatus = order.Status,
                            NewStatus = order.Status,
                            OldWarehouseId = order.AssignedWarehouseId,
                            NewWarehouseId = order.AssignedWarehouseId,
                            DateCreated = DateTime.Now,
                            Comments = String.Format("Автоматическое создание заявки на дозакуп №{0} на пользователя {1}",newFeaOrder.Id,targetManager.GetFio())
                        });
                        DataContext.SubmitChanges();
                        ShowSuccess(string.Format("Заявка на дозакуп №{0} успешно сформирована", newFeaOrder.Id));
                    }
                }
            }

            return Redirect(redirectUrl + "#history");
        }
    }


}
