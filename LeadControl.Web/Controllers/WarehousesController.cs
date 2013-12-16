using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Enums;
using LeadControl.Domain.Routing;
using LeadControl.Web.Classes.Security;
using Newtonsoft.Json.Schema;

namespace LeadControl.Web.Controllers
{
    /// <summary>
    /// Контроллер управления остатками на складах
    /// </summary>
    public class WarehousesController : BaseController
    {
        /// <summary>
        /// Отображает список складов к которым пользователь имеет доступ. Склады группируются по проекту
        /// </summary>
        /// <returns></returns>
        [Route("warehouses")][AuthorizationCheck(Permission.Warehousing)]
        public ActionResult Index()
        {
            var warehouses = CurrentUser.IsAdmin()
                ? DataContext.Warehouses.ToList()
                : CurrentUser.WarehouseKeepers.Select(wk => wk.Warehouse).ToList();

            PushNavigationItem("Склады","/warehouses");
            PushNavigationItem("Список складов","#");

            return View(warehouses);
        }

        /// <summary>
        /// Отображает страницу редактирования остатков по указанному складу
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("warehouses/{id}/edit")][AuthorizationCheck(Permission.Warehousing)]
        public ActionResult Edit(long id)
        {
            var warehouses = CurrentUser.IsAdmin()
                ? DataContext.Warehouses.Select(w => w.Id)
                : CurrentUser.WarehouseKeepers.Select(w => w.WarehouseId);

            // Ищес склад
            var warehouse = DataContext.Warehouses.FirstOrDefault(w => w.Id == id && warehouses.Contains(id));
            if (warehouse == null)
            {
                ShowError("Такой склад не найден");
                return RedirectToAction("Index");
            }

            PushNavigationItem("Склады", "/warehouses");
            PushNavigationItem("Редактирование остатков склада "+warehouse.Title, "#");

            return View(warehouse);
        }

        /// <summary>
        /// Обрабатывает изменение количества остатков указанного товара на указанном складе
        /// </summary>
        /// <param name="id">Идентификатор склада</param>
        /// <param name="pid">Идентификатор типа товара</param>
        /// <param name="newVal">Новое количество остатка</param>
        /// <returns></returns>
        [Route("warehouses/change-quantity")][AuthorizationCheck(Permission.Warehousing)][HttpPost]
        public ActionResult ChangeQuantity(long id, long pid, int newVal)
        {
            // Доступные склады
            var warehouses = CurrentUser.IsAdmin()
                ? DataContext.Warehouses.ToList()
                : CurrentUser.WarehouseKeepers.Select(wk => wk.Warehouse).ToList();

            // Ищем склад и тип продукта
            var warehouse = warehouses.FirstOrDefault(w => w.Id == id);
            if (warehouse == null)
            {
                return Json(new {success = false, msg = "Такой склад не найден"});
            }
            if (warehouse.Project.ProductTypes.All(p => p.Id != pid))
            {
                return Json(new { success = false, msg = "Такой тип продукта не найден" });
            }

            if (newVal < 0)
            {
                newVal = 0;
            }

            var warehouseProduct = warehouse.WarehouseProducts.FirstOrDefault(wp => wp.ProductId == pid);
            if (warehouseProduct == null)
            {
                // Создаем информацию об остатках товара на складе
                warehouseProduct = new WarehouseProduct()
                {
                    DateCreated = DateTime.Now,
                    Price = 0,
                    ProductId = pid,
                    Warehouse = warehouse,
                    Quantity = newVal
                };
                warehouse.WarehouseProducts.Add(warehouseProduct);
                if (newVal > 0)
                {
                    warehouseProduct.WarehouseProductChangements.Add(new WarehouseProductChangement()
                    {
                        Amount = newVal,
                        DateCreated = DateTime.Now,
                        Direction = (short)WarehouseProductChangementDirection.Income,
                        WarehouseProduct = warehouseProduct,
                        Description = String.Format("Ручное изменение: добавление {0} шт. пользователем {1}",newVal,CurrentUser.GetFio())
                    });
                }
            }
            else
            {
                // Изменяем
                var diff = newVal - warehouseProduct.Quantity;
                if (diff != 0)
                {
                    warehouseProduct.Quantity = newVal;
                    warehouseProduct.DateModified = DateTime.Now;
                    warehouseProduct.WarehouseProductChangements.Add(new WarehouseProductChangement()
                    {
                        Amount = diff,
                        DateCreated = DateTime.Now,
                        WarehouseProduct = warehouseProduct,
                        Direction = diff > 0 ? (short)WarehouseProductChangementDirection.Income : (short)WarehouseProductChangementDirection.Outcome,
                        Description = diff > 0 ? String.Format("Ручное изменение: добавление {0} шт. пользователем {1}",diff,CurrentUser.GetFio()) : String.Format("Ручное изменение: изъятие {0} шт. пользователем {1}",Math.Abs(diff),CurrentUser.GetFio())
                    });
                }
            }

            // Сохраняем
            try
            {
                DataContext.SubmitChanges();
            }
            catch (Exception e)
            {
                return Json(new { success = false, msg = "Ошибка сохранения в базу: "+e.Message });
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// Изменяет цену указанного товара на указанном складу
        /// </summary>
        /// <param name="id">Идентификатор склада</param>
        /// <param name="pid">Идентификатор типа товара</param>
        /// <param name="newVal">Новое значение цены</param>
        /// <returns></returns>
        [Route("warehouses/change-price")]
        [AuthorizationCheck(Permission.Warehousing)]
        [HttpPost]
        public ActionResult ChangePrice(long id, long pid, decimal newVal)
        {
            // Доступные склады
            var warehouses = CurrentUser.IsAdmin()
                ? DataContext.Warehouses.ToList()
                : CurrentUser.WarehouseKeepers.Select(wk => wk.Warehouse).ToList();

            // Ищем склад и тип продукта
            var warehouse = warehouses.FirstOrDefault(w => w.Id == id);
            if (warehouse == null)
            {
                return Json(new { success = false, msg = "Такой склад не найден" });
            }
            if (warehouse.Project.ProductTypes.All(p => p.Id != pid))
            {
                return Json(new { success = false, msg = "Такой тип продукта не найден" });
            }

            if (newVal < 0)
            {
                newVal = 0;
            }

            // Ищем или создаем
            var warehouseProduct = warehouse.WarehouseProducts.FirstOrDefault(wp => wp.ProductId == pid);
            if (warehouseProduct == null)
            {
                warehouseProduct = new WarehouseProduct()
                {
                    DateCreated = DateTime.Now,
                    Price = 0,
                    Quantity = 0,
                    ProductId = pid,
                    Warehouse = warehouse
                };
                warehouse.WarehouseProducts.Add(warehouseProduct);
            }
            warehouseProduct.Price = newVal;
            warehouseProduct.DateModified = DateTime.Now;

            // Сохраняем
            try
            {
                DataContext.SubmitChanges();
            }
            catch (Exception e)
            {
                return Json(new { success = false, msg = "Ошибка сохранения в базу: " + e.Message });
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// Отображает таблицу с историей изменения позиции указанного товара
        /// </summary>
        /// <param name="id">Идентификатор позиции</param>
        /// <returns></returns>
        [Route("warehouses/history/{id}")][AuthorizationCheck(Permission.Warehousing)]
        public ActionResult History(long id)
        {
            // Доступные склады
            var warehouses = CurrentUser.IsAdmin()
                ? DataContext.Warehouses.ToList()
                : CurrentUser.WarehouseKeepers.Select(wk => wk.Warehouse).ToList();

            // Все позиции со всех складов
            var positions = from warehouse in warehouses
                from position in warehouse.WarehouseProducts
                select position;

            // Искомая позиция
            var item = positions.FirstOrDefault(p => p.Id == id);
            if (item == null)
            {
                ShowError("Такая позиция не найдена");
                return RedirectToAction("Index");
            }

            PushNavigationItem("Склады", "/warehouses");
            PushNavigationItem("Редактирование остатков склада " + item.Warehouse.Title, string.Format("/warehouses/{0}/edit", item.WarehouseId));
            PushNavigationItem("История изменения остатков " + item.ProductType.Title, "#");

            return View(item);
        }

    }
}
