using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Enums;
using LeadControl.Domain.Routing;
using LeadControl.Web.Classes.Security;
using LeadControl.Web.Models.FEA;

namespace LeadControl.Web.Controllers
{
    /// <summary>
    /// Контроллер ВЭД заявок
    /// </summary>
    public class FEAController : BaseController
    {
        /// <summary>
        /// Отображает список ВЭД заявок
        /// </summary>
        /// <returns></returns>
        [Route("fea")][AuthorizationCheck(Permission.FEA)]
        public ActionResult Index(FEAFiltrationModel model)
        {
            // Фильтруем
            var availableProjects = CurrentUser.IsAdmin() ? DataContext.Projects.Select(p => p.Id) : CurrentUser.ProjectUsers.Select(pu => pu.ProjectId);
            IEnumerable<FEAOrder> orders = CurrentUser.IsAdmin()
                ? DataContext.FEAOrders
                : DataContext.FEAOrders.Where(fo => availableProjects.Contains(fo.ProjectId));
            if (model.ProjectIds.Length > 0)
            {
                orders = orders.Where(fo => model.ProjectIds.Contains(fo.ProjectId));
            }
            if (model.Statuses.Length > 0)
            {
                orders = orders.Where(fo => model.Statuses.Contains(fo.Status));
            }
            if (model.UserIds.Length > 0)
            {
                orders = orders.Where(fo => model.UserIds.Contains(fo.ManagerId));
            }
            if (!String.IsNullOrEmpty(model.Term))
            {
                var term = model.Term.ToLower();
                orders =
                    orders.Where(
                        o =>
                            (o.Description != null && o.Description.ToLower().Contains(term)) ||
                            o.FEAOrderItems.Any(oi => oi.ProductType.Title.ToLower().Contains(term)));
            }

            model.Fetched = orders.AsEnumerable().OrderByDescending(o => o.LastUpdate).ToList();

            PushNavigationItem("ВЭД","/fea");
            PushNavigationItem("Список ВЭД заявок","#");

            // Отдаем вид
            return View(model);
        }

        /// <summary>
        /// Отображает форму создания новой заявки
        /// </summary>
        /// <returns></returns>
        [Route("fea/create")][AuthorizationCheck(Permission.FEA)]
        public ActionResult Create()
        {
            PushNavigationItem("ВЭД", "/fea");
            PushNavigationItem("Создание новой заявки", "#");

            return View();
        }

        /// <summary>
        /// Создает ВЭД заявку и перенаправляет на форму редактирования позиций в ней
        /// </summary>
        /// <param name="model">Модель данных</param>
        /// <returns></returns>
        [Route("fea/create-order")][HttpPost][AuthorizationCheck(Permission.FEA)]
        public ActionResult DoCreate(FEAOrder model)
        {
            // Создаем
            model.DateCreated = DateTime.Now;
            model.Status = (short) FEAOrderStatus.Gathering;
            DataContext.FEAOrders.InsertOnSubmit(model);
            DataContext.SubmitChanges();

            ShowSuccess(string.Format("Новая заявка №{0} успешно создана", model.Id));

            return Redirect(string.Format("/fea/{0}/edit#items", model.Id));
        }

        /// <summary>
        /// Отображает карточку редактирования ВЭД заявки
        /// </summary>
        /// <param name="id">Идентификатор заявки</param>
        /// <returns></returns>
        [Route("fea/{id}/edit")][AuthorizationCheck(Permission.FEA)]
        public ActionResult Edit(long id)
        {
            // Ищем заявку
            var availableProjects = CurrentUser.IsAdmin() ? DataContext.Projects.Select(p => p.Id) : CurrentUser.ProjectUsers.Select(pu => pu.ProjectId);
            var order = DataContext.FEAOrders.FirstOrDefault(o => o.Id == id && availableProjects.Contains(o.ProjectId));
            if (order == null)
            {
                ShowError("Такая заявка не найдена");
                return RedirectToAction("Index");
            }

            PushNavigationItem("ВЭД", "/fea");
            PushNavigationItem("Карточка заявки №"+order.Id, "#");

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
        [HttpPost][Route("fea/change-order-item-product")][AuthorizationCheck(Permission.FEA)]
        public ActionResult ChangeOrderItemProduct(long id, long itemId, long productId, long newId)
        {
            var order = DataContext.FEAOrders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Заявка не найдена"
                });
            }

            // Ищем позицию
            FEAOrderItem orderItem;
            if (productId == 0 && itemId == 0)
            {
                orderItem = new FEAOrderItem()
                {
                    DateCreated = DateTime.Now,
                    FEAOrder = order,
                    Price = 0,
                    Quantity = 0,
                    ProductId = newId
                };
                order.FEAOrderItems.Add(orderItem);
            }
            else
            {
                orderItem = order.FEAOrderItems.FirstOrDefault(oi => oi.Id == itemId);
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
                orderItem.ProductType.FEAOrderItems.Remove(orderItem);
                productType.FEAOrderItems.Add(orderItem);
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
        /// <param name="quantity">Новый идентификатор продукта</param>
        /// <returns></returns>
        [HttpPost][Route("fea/change-order-item-quantity")][AuthorizationCheck(Permission.FEA)]
        public ActionResult ChangeOrderItemQuantity(long id, long itemId, long productId, int quantity)
        {
            var order = DataContext.FEAOrders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Заявка не найдена"
                });
            }

            // Ищем позицию
            FEAOrderItem orderItem;
            if (productId == 0 && itemId == 0)
            {
                orderItem = new FEAOrderItem()
                {
                    DateCreated = DateTime.Now,
                    FEAOrder = order,
                    Price = 0,
                    Quantity = quantity,
                    ProductId = order.Project.ProductTypes.First().Id
                };
                order.FEAOrderItems.Add(orderItem);
            }
            else
            {
                orderItem = order.FEAOrderItems.FirstOrDefault(oi => oi.Id == itemId);
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
        [HttpPost][Route("fea/change-order-item-price")][AuthorizationCheck(Permission.FEA)]
        public ActionResult ChangeOrderItemPrice(long id, long itemId, long productId, decimal price)
        {
            var order = DataContext.FEAOrders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Заявка не найдена"
                });
            }

            // Ищем позицию
            FEAOrderItem orderItem;
            if (productId == 0 && itemId == 0)
            {
                orderItem = new FEAOrderItem()
                {
                    DateCreated = DateTime.Now,
                    FEAOrder = order,
                    Price = price,
                    Quantity = 0,
                    ProductId = order.Project.ProductTypes.First().Id
                };
                order.FEAOrderItems.Add(orderItem);
            }
            else
            {
                orderItem = order.FEAOrderItems.FirstOrDefault(oi => oi.Id == itemId);
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
        [Route("fea/delete-order-item")]
        [AuthorizationCheck(Permission.FEA)]
        public ActionResult DeleteOrderItem(long id, long itemId)
        {
            // Ищем заказ
            var order = DataContext.FEAOrders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Заявка не найдена"
                });
            }

            // Ищем позицию
            var orderItem = order.FEAOrderItems.FirstOrDefault(oi => oi.Id == itemId);
            if (orderItem == null)
            {
                return Json(new
                {
                    success = false,
                    msg = "Такая позиция не найдена"
                });
            }

            // Удаляем
            DataContext.FEAOrderItems.DeleteOnSubmit(orderItem);

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

            return Json(new {success = true});
        }

        /// <summary>
        /// Изменяет менеджера, назначенного на эту ВЭД заявку
        /// </summary>
        /// <param name="id">Идентификатор заявки</param>
        /// <param name="managerId">Идентификатор нового менеджера</param>
        /// <returns></returns>
        [Route("fea/change-manager")][HttpPost][AuthorizationCheck(Permission.FEA)]
        public ActionResult ChangeOrderManager(long id, long managerId)
        {
            // Ищем заявку
            var availableProjects = CurrentUser.IsAdmin() ? DataContext.Projects.Select(p => p.Id) : CurrentUser.ProjectUsers.Select(pu => pu.ProjectId);
            var order = DataContext.FEAOrders.FirstOrDefault(o => o.Id == id && availableProjects.Contains(o.ProjectId));
            if (order == null)
            {
                ShowError("Такая заявка не найдена");
                return RedirectToAction("Index");
            }

            if (managerId != order.ManagerId)
            {
                var newManager = DataContext.Users.FirstOrDefault(m => m.Id == managerId);
                if (newManager == null)
                {
                    ShowError("Такой менеджер не найден");
                    return RedirectToAction("Index");    
                }

                order.Manager.FEAOrders.Remove(order);
                newManager.FEAOrders.Add(order);
                DataContext.SubmitChanges();

                ShowSuccess(string.Format("Менеджером заявки №{0} назначен {1}", id, newManager.GetFioSmall()));
            }

            return RedirectToAction("Edit", new {id});
        }

        /// <summary>
        /// Обновляет изменение статуса указанной ВЭД заявки
        /// </summary>
        /// <param name="id">Идентификатор заявки</param>
        /// <param name="newStatus">Новый статус</param>
        /// <param name="comments">Комментарии к изменению</param>
        /// <param name="updateWarehouse">Обновить остатки на указанном складе</param>
        /// <returns></returns>
        [Route("fea/change-order-status")][HttpPost][AuthorizationCheck(Permission.FEA)]
        public ActionResult UpdateOrderStatus(long id, short newStatus, string comments, bool updateWarehouse)
        {
            // Ищем заявку
            var availableProjects = CurrentUser.IsAdmin() ? DataContext.Projects.Select(p => p.Id) : CurrentUser.ProjectUsers.Select(pu => pu.ProjectId);
            var order = DataContext.FEAOrders.FirstOrDefault(o => o.Id == id && availableProjects.Contains(o.ProjectId));
            if (order == null)
            {
                ShowError("Такая заявка не найдена");
                return RedirectToAction("Index");
            }

            // Проверяем можем ли мы изменить статус
            if (!order.CanChangeStatus(CurrentUser))
            {
                ShowError("Нельзя изменить статус у этой заявки");
                return RedirectToAction("Index");
            }

            // Изменяем статус
            if (newStatus != order.Status)
            {
                order.Status = newStatus;
                order.FEAOrdersStatusChangements.Add(new FEAOrdersStatusChangement()
                {
                    Status = newStatus,
                    FEAOrder = order,
                    Comments = comments,
                    User = CurrentUser,
                    DateCreated = DateTime.Now
                });

                ShowSuccess("Статус ВЭД заявки №{0} успешно изменен");

                // Провряем, требуется ли обновить остатки на указанному складе
                if (newStatus == (short) FEAOrderStatus.Completed && updateWarehouse && order.TargetWarehouse != null)
                {
                    // Обновляем остатки на складе
                    foreach (var orderItem in order.FEAOrderItems)
                    {
                        // Ищем позицию на складе
                        var warehouseItem =
                            order.TargetWarehouse.WarehouseProducts.FirstOrDefault(
                                wp => wp.ProductId == orderItem.ProductId);

                        // Создаем если ее нет
                        if (warehouseItem == null)
                        {
                            warehouseItem = new WarehouseProduct()
                            {
                                ProductId = orderItem.ProductId,
                                DateCreated = DateTime.Now,
                                WarehouseId = order.TargetWarehouse.Id,
                                Price = orderItem.Price.HasValue ? orderItem.Price.Value : 0,
                            };
                            order.TargetWarehouse.WarehouseProducts.Add(warehouseItem);
                        }

                        // Добавляем начисление
                        warehouseItem.Quantity = warehouseItem.Quantity + orderItem.Quantity;
                        warehouseItem.WarehouseProductChangements.Add(new WarehouseProductChangement()
                        {
                            Amount = orderItem.Quantity,
                            Direction = (short)WarehouseProductChangementDirection.Income,
                            DateCreated = DateTime.Now,
                            WarehouseProduct = warehouseItem,
                            Description = String.Format("Автоматическое начисление остатков по выполнению ВЭД заявки №{0}",order.Id)
                        });
                    }

                    ShowSuccess(string.Format("На склад {0} ({1}) успешно зачислены товары из ВЭД заявки №{2}", order.TargetWarehouse.Title, order.TargetWarehouse.City, order.Id));
                }

                DataContext.SubmitChanges();
            }

            return Redirect(string.Format("/fea/{0}/edit#history", order.Id));
        }
    }
}
