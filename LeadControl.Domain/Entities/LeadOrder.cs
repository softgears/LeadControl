// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: LeadOrder.cs
// 
// Created by: ykors_000 at 05.12.2013 15:14
// 
// Property of SoftGears
// 
// ========

using System.Linq;

namespace LeadControl.Domain.Entities
{
    /// <summary>
    /// Заказ у лида
    /// </summary>
    public partial class LeadOrder
    {
        /// <summary>
        /// Возвращает общую сумму заказа
        /// </summary>
        /// <returns></returns>
        public decimal GetOrderTotalPrice()
        {
            return LeadOrderItems.Sum(oi => oi.Price*oi.Quantity);
        }
    }
}