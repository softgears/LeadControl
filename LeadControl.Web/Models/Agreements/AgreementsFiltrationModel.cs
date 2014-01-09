// 
// 
// Solution: LeadControl
// Project: LeadControl.Web
// File: AgreementsFiltrationModel.cs
// 
// Created by: ykors_000 at 09.01.2014 15:07
// 
// Property of SoftGears
// 
// ========

using LeadControl.Domain.Entities;

namespace LeadControl.Web.Models.Agreements
{
    /// <summary>
    /// Модель фильтрации договоров
    /// </summary>
    public class AgreementsFiltrationModel: BaseFiltrationModel<LeadAgreement>
    {
        /// <summary>
        /// Проекты, к которым принадлежат услуги
        /// </summary>
        public long[] ProjectIds { get; set; }

        /// <summary>
        /// Идентификаторы лидов
        /// </summary>
        public long[] LeadIds { get; set; }

        /// <summary>
        /// Статусы договоров
        /// </summary>
        public short[] Statuses { get; set; }

        /// <summary>
        /// Назначенные менеджеры
        /// </summary>
        public long[] Managers { get; set; }

        /// <summary>
        /// Типы услуг в договоре
        /// </summary>
        public long[] ServiceTypes { get; set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public AgreementsFiltrationModel()
        {
            ProjectIds = new long[]{};
            LeadIds = new long[]{};
            Statuses = new short[]{};
            Managers = new long[]{};
            ServiceTypes = new long[]{};
        }
    }
}