// 
// 
// Solution: LeadControl
// Project: LeadControl.Web
// File: OrdersFiltrationModel.cs
// 
// Created by: ykors_000 at 19.12.2013 12:39
// 
// Property of SoftGears
// 
// ========

using LeadControl.Domain.Entities;

namespace LeadControl.Web.Models.Orders
{
    /// <summary>
    /// Модель фильтрации заявок
    /// </summary>
    public class OrdersFiltrationModel: BaseFiltrationModel<LeadOrder>
    {
        /// <summary>
        /// Идентификаторы проектов
        /// </summary>
        public long[] ProjectIds { get; set; }

        /// <summary>
        /// Идентификаторы пользователей, которые участвовали в заявке
        /// </summary>
        public long[] UsersIds { get; set; }

        /// <summary>
        /// Статусы, в которых находится заявка
        /// </summary>
        public short[] Statuses { get; set; }

        /// <summary>
        /// Типы доставок
        /// </summary>
        public short[] DeliveryTypes { get; set; }

        /// <summary>
        /// Типы оплат
        /// </summary>
        public short[] PaymentTypes { get; set; }

        /// <summary>
        /// Идентификаторы типов продуктов, которые есть в заявках
        /// </summary>
        public long[] ProductTypesIds { get; set; }

        /// <summary>
        /// Заявка полностью оплачена
        /// </summary>
        public bool Payed { get; set; }

        /// <summary>
        /// Идентификаторы лидов
        /// </summary>
        public long[] LeadIds { get; set; }

        public OrdersFiltrationModel()
        {
            ProjectIds = new long[]{};
            UsersIds = new long[]{};
            Statuses = new short[]{};
            DeliveryTypes = new short[]{};
            PaymentTypes = new short[]{};
            ProductTypesIds = new long[]{};
            LeadIds = new long[]{};
        }
    }
}