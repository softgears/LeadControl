// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: LeadAgreement.cs
// 
// Created by: ykors_000 at 10.01.2014 13:10
// 
// Property of SoftGears
// 
// ========

using LeadControl.Domain.Enums;

namespace LeadControl.Domain.Entities
{
    /// <summary>
    /// Договор на оказание услуг
    /// </summary>
    public partial class LeadAgreement
    {
        /// <summary>
        /// Получает общую прибыль, которая возможна по данному договору
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalPlannedAmount()
        {
            decimal total = 0;
            foreach (var item in LeadAgreementServices)
            {
                if (item.ServiceType.Type == (short) LeadServiceTypes.Periodical)
                {
                    total += item.Price*item.Period;
                }
                else
                {
                    total += item.Price;
                }
            }
            return total;
        }
    }
}