// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: ServiceType.cs
// 
// Created by: ykors_000 at 10.01.2014 15:43
// 
// Property of SoftGears
// 
// ========

using System;
using LeadControl.Domain.Enums;

namespace LeadControl.Domain.Entities
{
    /// <summary>
    /// Тип услуги в системе
    /// </summary>
    public partial class ServiceType
    {
        /// <summary>
        /// Вычисляет количество период оказания услуги исходя из данных договора
        /// </summary>
        /// <param name="agreement">Договор</param>
        /// <returns></returns>
        public int ComputeTotalPeriods(LeadAgreement agreement)
        {
            var startDate = agreement.Date.Value;
            var endDate = agreement.EndDate ?? startDate.AddYears(1);
            if (Type == (short) LeadServiceTypes.OneShot)
            {
                return 1;
            }
            var diff = endDate - startDate;
            int totalDays = (int) diff.TotalDays;
            int result = 0;
            switch ((LeadServicePeriods)PeriodType)
            {
                case LeadServicePeriods.No:
                    result =  1;
                    break;
                case LeadServicePeriods.Daily:
                    result = totalDays;
                    break;
                case LeadServicePeriods.Weekly:
                    result = totalDays/7;
                    break;
                case LeadServicePeriods.Montly:
                    result = totalDays/30;
                    break;
                case LeadServicePeriods.Quartal:
                    result = totalDays/30/3;
                    break;
                case LeadServicePeriods.HalfYearly:
                    result = totalDays/30/6;
                    break;
                case LeadServicePeriods.Yearly:
                    result = totalDays/365;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (result <= 0)
            {
                result = 1;
            }
            return result;
        }
    }
}