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
using LeadControl.Domain.Enums;

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

        /// <summary>
        /// Проверяет, может ли указанный пользователь изменять статус заявки
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CanChangeStatus(User user)
        {
            if (user.IsAdmin())
            {
                return true;
            }

            if (ManagerId == user.Id && Status != (short) FEAOrderStatus.Completed)
            {
                return true;
            }

            return false;
        }
    }
}