// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: LeadServicePaymentTypes.cs
// 
// Created by: ykors_000 at 09.01.2014 12:10
// 
// Property of SoftGears
// 
// ========
namespace LeadControl.Domain.Enums
{
    /// <summary>
    /// Типы оплаты по услугам
    /// </summary>
    public enum LeadServicePaymentTypes: short
    {
        /// <summary>
        /// Полная предоплата за услугу
        /// </summary>
        [EnumDescription("Полная предоплата")]
        FullPrepayment = 1,

        /// <summary>
        /// Предоплата 50% за услугу
        /// </summary>
        [EnumDescription("50% предоплата")]
        HalfPrepayment = 2,

        /// <summary>
        /// Оплата после оказания услги
        /// </summary>
        [EnumDescription("Постоплата")]
        PostPayment = 3,

        /// <summary>
        /// Сдельная почасовая
        /// </summary>
        [EnumDescription("Сдельная почасовая")]
        BasedOnTime = 4,
    }
}