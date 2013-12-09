// 
// 
// Solution: LeadControl
// Project: LeadControl.Web
// File: LeadsFiltrationModel.cs
// 
// Created by: ykors_000 at 09.12.2013 11:09
// 
// Property of SoftGears
// 
// ========

using LeadControl.Domain.Entities;

namespace LeadControl.Web.Models.Leads
{
    /// <summary>
    /// Модель для фильтрации списка лидов
    /// </summary>
    public class LeadsFiltrationModel: BaseFiltrationModel<Lead>
    {
        /// <summary>
        /// Идентификаторы проектов, заказы по которым есть у лидов
        /// </summary>
        public long[] ProjectIds { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public LeadsFiltrationModel()
        {
            ProjectIds = new long[]{};
        }
    }
}