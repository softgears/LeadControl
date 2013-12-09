// 
// 
// Solution: LeadControl
// Project: LeadControl.Web
// File: WarehousesFiltrationModel.cs
// 
// Created by: ykors_000 at 09.12.2013 14:28
// 
// Property of SoftGears
// 
// ========

using LeadControl.Domain.Entities;

namespace LeadControl.Web.Models.Manage
{
    /// <summary>
    /// Модель фильтрации складов
    /// </summary>
    public class WarehousesFiltrationModel: BaseFiltrationModel<Warehouse>
    {
        /// <summary>
        /// Проекты, к которым принадлежат товары
        /// </summary>
        public long[] ProjectIds { get; set; }

        public WarehousesFiltrationModel()
        {
            ProjectIds = new long[]{};
        }
    }
}