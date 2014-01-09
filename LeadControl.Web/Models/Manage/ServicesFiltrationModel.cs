// 
// 
// Solution: LeadControl
// Project: LeadControl.Web
// File: ServicesFiltrationModel.cs
// 
// Created by: ykors_000 at 09.01.2014 12:18
// 
// Property of SoftGears
// 
// ========

using LeadControl.Domain.Entities;

namespace LeadControl.Web.Models.Manage
{
    /// <summary>
    /// Модель фильтрации типов услуг
    /// </summary>
    public class ServicesFiltrationModel: BaseFiltrationModel<ServiceType>
    {
        /// <summary>
        /// Проекты, к которым принадлежат услуги
        /// </summary>
        public long[] ProjectIds { get; set; }

        /// <summary>
        /// Типы услуг по признаку оказания
        /// </summary>
        public short[] ServiceTypes { get; set; }

        /// <summary>
        /// Периодичность оказания услуг
        /// </summary>
        public short[] ServicePeriods { get; set; }

        /// <summary>
        /// Типы оплаты услуги
        /// </summary>
        public short[] PaymentTypes { get; set; }

        public ServicesFiltrationModel()
        {
            ProjectIds = new long[]{};
            ServiceTypes = new short[]{};
            ServicePeriods = new short[]{};
            PaymentTypes = new short[]{};
        }
    }
}