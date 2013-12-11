// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: FEAOrder.cs
// 
// Created by: ykors_000 at 11.12.2013 11:42
// 
// Property of SoftGears
// 
// ========

using System;
using System.Linq;

namespace LeadControl.Domain.Entities
{
    /// <summary>
    /// ВЭД заявка
    /// </summary>
    public partial class FEAOrder
    {
        /// <summary>
        /// Дата и время последнего обновления заявки
        /// </summary>
        public DateTime LastUpdate
        {
            get
            {
                return FEAOrdersStatusChangements.Count == 0
                    ? DateCreated.Value
                    : FEAOrdersStatusChangements.OrderByDescending(fo => fo.DateCreated).First().DateCreated.Value;
            }
        }
    }
}