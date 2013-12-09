using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Routing;
using LeadControl.Web.Classes.Security;
using LeadControl.Web.Models.Manage;

namespace LeadControl.Web.Controllers
{
    /// <summary>
    /// Контроллер управления складами
    /// </summary>
    public class ManageWarehousesController : BaseController
    {
        /// <summary>
        /// Отображает список зарегистрированных в системе складов
        /// </summary>
        /// <returns></returns>
        [Route("manage/warehouses")][AuthorizationCheck(Permission.ManageWarehouses)]
        public ActionResult Index(WarehousesFiltrationModel model)
        {
            var availableProjects = CurrentUser.ProjectUsers.Select(p => p.ProjectId).ToArray();
            IEnumerable<Warehouse> warehouses = DataContext.Warehouses.Where(w => availableProjects.Contains(w.ProjectId));
            if (model.ProjectIds.Length > 0)
            {
                warehouses = warehouses.Where(w => model.ProjectIds.Contains(w.ProjectId));
            }
            if (!String.IsNullOrEmpty(model.Term))
            {
                var term = model.Term.ToLower();
                warehouses = warehouses.Where(w => (w.Title.ToLower().Contains(term)) ||
                                                   (w.Address != null && w.Address.ToLower().Contains(term)) ||
                                                   (w.Description != null && w.Description.ToLower().Contains(term)) ||
                                                   (w.Description != null && w.City.ToLower().Contains(term)));
            }

            model.Fetched = warehouses.OrderBy(w => w.Title).ToList();

            PushNavigationItem("Управление складами","/manage/warehouses");
            PushNavigationItem("Список складов","#");

            return View(model);
        }

        /// <summary>
        /// Отображает форму создания нового склада
        /// </summary>
        /// <returns></returns>
        [Route("manage/warehouses/add")]
        [AuthorizationCheck(Permission.ManageWarehouses)]
        public ActionResult Add()
        {
            PushNavigationItem("Управление складами", "/manage/warehouses");
            PushNavigationItem("Новый склад", "#");

            return View("WarehouseEditForm", new Warehouse()
            {

            });
        }

        /// <summary>
        /// Отображает форму редактирования склада
        /// </summary>
        /// <param name="id">Идентификатор типа товара</param>
        /// <returns></returns>
        [Route("manage/warehouses/{id}/edit")]
        [AuthorizationCheck(Permission.ManageWarehouses)]
        public ActionResult Edit(long id)
        {
            var warehouse = DataContext.Warehouses.FirstOrDefault(p => p.Id == id);
            if (warehouse == null)
            {
                ShowError("Такой склад не найден");
                return RedirectToAction("Index");
            }

            PushNavigationItem("Управление складами", "/manage/warehouses");
            PushNavigationItem("Редактирование склада " + warehouse.Title, "#");

            return View("WarehouseEditForm", warehouse);
        }

        /// <summary>
        /// Удаляет указанный  склад и все остатки товаров в нем
        /// </summary>
        /// <param name="id">Идентификатор типа товара</param>
        /// <returns></returns>
        [Route("manage/warehouses/{id}/delete")]
        [AuthorizationCheck(Permission.ManageWarehouses)]
        public ActionResult Delete(long id)
        {
            var warehouse = DataContext.Warehouses.FirstOrDefault(p => p.Id == id);
            if (warehouse == null)
            {
                ShowError("Такой склад не найден");
                return RedirectToAction("Index");
            }

            DataContext.Warehouses.DeleteOnSubmit(warehouse);
            DataContext.SubmitChanges();
            ShowSuccess("Склад был успешно удален");

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Обрабатывает создание или редактирование склада
        /// </summary>
        /// <param name="model">Модель данных типа товара</param>
        /// <param name="collection">Коллекция данных форм</param>
        /// <returns></returns>
        [HttpPost]
        [Route("manage/warehouses/save")]
        [AuthorizationCheck(Permission.ManageWarehouses)]
        public ActionResult Save(Warehouse model, FormCollection collection)
        {
            Warehouse warehouse;
            if (model.Id <= 0)
            {
                model.DateCreated = DateTime.Now;
                DataContext.Warehouses.InsertOnSubmit(model);
                DataContext.SubmitChanges();

                warehouse = model;

                ShowSuccess("Склад был успешно создан");
            }
            else
            {
                warehouse = DataContext.Warehouses.FirstOrDefault(p => p.Id == model.Id);
                if (warehouse == null)
                {
                    ShowSuccess("Такой склад не найден");
                    return RedirectToAction("Index");
                }

                warehouse.Title = model.Title;
                warehouse.Description = model.Description;
                warehouse.City = model.City;
                warehouse.Address = model.Address;
                warehouse.DateModified = DateTime.Now;
                if (model.ProjectId != warehouse.ProjectId)
                {
                    warehouse.Project.Warehouses.Remove(warehouse);
                    DataContext.Projects.First(p => p.Id == model.Id).Warehouses.Add(warehouse);
                }
                DataContext.SubmitChanges();

                ShowSuccess(string.Format("Склад {0} был успешно отредактирован", model.Title));
            }

            // Обновляем пермишенны для роли
            warehouse.WarehouseKeepers.Clear();
            foreach (var userId in collection.AllKeys.Where(k => k.StartsWith("user_")).Select(key => Convert.ToInt64(key.Split('_')[1])))
            {
                warehouse.WarehouseKeepers.Add(new WarehouseKeeper()
                {
                    Warehouse = warehouse,
                    UserId = userId,
                    DateCreated = DateTime.Now
                });
            }
            DataContext.SubmitChanges();

            return RedirectToAction("Index");
        }
    }
}
