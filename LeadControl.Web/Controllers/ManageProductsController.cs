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
    /// Контроллер управления типами товаров
    /// </summary>
    public class ManageProductsController : BaseController
    {
        /// <summary>
        /// Контроллер управления типами товаров
        /// </summary>
        /// <returns></returns>
        [Route("manage/products")][AuthorizationCheck(Permission.ManageProducts)]
        public ActionResult Index(ProductsFiltrationModel model)
        {
            // Фильтруем
            IEnumerable<ProductType> products = DataContext.ProductTypes;
            if (model.ProjectIds.Length > 0)
            {
                products = products.Where(p => model.ProjectIds.Contains(p.ProjectId));
            }
            if (!String.IsNullOrEmpty(model.Term))
            {
                var term = model.Term.ToLower();
                products =
                    products.Where(
                        p =>
                            p.Article.ToLower().Contains(term) || p.Title.ToLower().Contains(term) ||
                            p.Description.ToLower().Contains(term));
            }
            if (model.InStock)
            {
                products = products.Where(p => p.WarehouseProducts.Any(wp => wp.Quantity > 0));
            }
            if (model.Ordered)
            {
                products =
                    products.Where(p => p.FEAOrderItems.Any(f => f.FEAOrder.Status != (short) FEAOrderStatus.Completed));
            }

            model.Fetched = products.OrderBy(p => p.Title).ToList();

            PushNavigationItem("Управление товарами","/manage/products");
            PushNavigationItem("Список товаров","#");

            return View(model);
        }

        /// <summary>
        /// Отображает форму создания нового типа товара
        /// </summary>
        /// <returns></returns>
        [Route("manage/products/add")][AuthorizationCheck(Permission.ManageProducts)]
        public ActionResult Add()
        {
            PushNavigationItem("Управление товарами", "/manage/products");
            PushNavigationItem("Новый тип товара", "#");

            return View("ProductEditForm", new ProductType()
            {

            });
        }

        /// <summary>
        /// Отображает форму редактирования типа товара
        /// </summary>
        /// <param name="id">Идентификатор типа товара</param>
        /// <returns></returns>
        [Route("manage/products/{id}/edit")][AuthorizationCheck(Permission.ManageProducts)]
        public ActionResult Edit(long id)
        {
            var product = DataContext.ProductTypes.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                ShowError("Такой тип товара не найден");
                return RedirectToAction("Index");
            }

            PushNavigationItem("Управление товарами", "/manage/products");
            PushNavigationItem("Редактирование товара "+product.Title, "#");

            return View("ProductEditForm", product);
        }

        /// <summary>
        /// Удаляет указанный тип товара, если с ним не было создано каких либо заказов или вэд заявок
        /// </summary>
        /// <param name="id">Идентификатор типа товара</param>
        /// <returns></returns>
        [Route("manage/products/{id}/delete")][AuthorizationCheck(Permission.ManageProducts)]
        public ActionResult Delete(long id)
        {
            var product = DataContext.ProductTypes.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                ShowError("Такой тип товара не найден");
                return RedirectToAction("Index");
            }

            if (product.LeadOrderItems.Count > 0)
            {
                ShowError("Нельзя удалить этот товар, потому что в системе есть заказы, его используюшие");
                return RedirectToAction("Index");
            }

            if (product.FEAOrderItems.Count > 0)
            {
                ShowError("Нельзя удалить этот товар, потому что в системе есть ВЭД заявки, его используюшие");
                return RedirectToAction("Index");
            }

            DataContext.ProductTypes.DeleteOnSubmit(product);
            DataContext.SubmitChanges();
            ShowSuccess("Товар был успешно удален");

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Обрабатывает создание или редактирование типа товара
        /// </summary>
        /// <param name="model">Модель данных типа товара</param>
        /// <returns></returns>
        [HttpPost][Route("manage/products/save")][AuthorizationCheck(Permission.ManageProducts)]
        public ActionResult Save(ProductType model)
        {
            if (model.Id <= 0)
            {
                model.DateCreated = DateTime.Now;
                DataContext.ProductTypes.InsertOnSubmit(model);
                DataContext.SubmitChanges();

                ShowSuccess("Товар был успешно создан");
            }
            else
            {
                var product = DataContext.ProductTypes.FirstOrDefault(p => p.Id == model.Id);
                if (product == null)
                {
                    ShowSuccess("Такой продукт не найден");
                    return RedirectToAction("Index");
                }

                product.Article = model.Article;
                product.Title = model.Title;
                product.Description = model.Description;
                product.MinAmount = model.MinAmount;
                product.Weight = model.Weight;
                product.DateModified = DateTime.Now;
                if (model.ProjectId != product.ProjectId)
                {
                    product.Project.ProductTypes.Remove(product);
                    DataContext.Projects.First(p => p.Id == model.Id).ProductTypes.Add(product);
                }
                DataContext.SubmitChanges();

                ShowSuccess(string.Format("Товар {0} был успешно отредактирован", model.Title));
            }

            return RedirectToAction("Index");
        }

    }
}
