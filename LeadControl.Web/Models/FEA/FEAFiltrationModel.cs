// 
// 
// Solution: LeadControl
// Project: LeadControl.Web
// File: FEAFiltrationModel.cs
// 
// Created by: ykors_000 at 11.12.2013 11:26
// 
// Property of SoftGears
// 
// ========

using LeadControl.Domain.Entities;
using LeadControl.Domain.Enums;

namespace LeadControl.Web.Models.FEA
{
    /// <summary>
    /// Модель фильтрации ВЭД заявок
    /// </summary>
    public class FEAFiltrationModel: BaseFiltrationModel<FEAOrder>
    {
        /// <summary>
        /// Идентификаторы проектов
        /// </summary>
        public long[] ProjectIds { get; set; }

        /// <summary>
        /// Статусы заявок
        /// </summary>
        public short[] Statuses { get; set; }

        /// <summary>
        /// Идентификаторы пользователей, на которых назначена заявка
        /// </summary>
        public long[] UserIds { get; set; }

        public FEAFiltrationModel()
        {
            ProjectIds = new long[] {};
            Statuses = new short[]{};
            UserIds = new long[]{};
        }
    }
}