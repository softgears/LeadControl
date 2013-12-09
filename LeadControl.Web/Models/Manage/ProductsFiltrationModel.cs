// 
// 
// Solution: LeadControl
// Project: LeadControl.Web
// File: ProductsFiltrationModel.cs
// 
// Created by: ykors_000 at 09.12.2013 13:01
// 
// Property of SoftGears
// 
// ========

using LeadControl.Domain.Entities;

namespace LeadControl.Web.Models.Manage
{
    /// <summary>
    /// Модель фильтрации типов товаров
    /// </summary>
    public class ProductsFiltrationModel: BaseFiltrationModel<ProductType>
    {
        /// <summary>
        /// Проекты, к которым принадлежат товары
        /// </summary>
        public long[] ProjectIds { get; set; }

        /// <summary>
        /// Товар в наличии
        /// </summary>
        public bool InStock { get; set; }

        /// <summary>
        /// Товар заказан и находится в пути
        /// </summary>
        public bool Ordered { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ProductsFiltrationModel()
        {
            ProjectIds = new long[]{};
        }
    }
}