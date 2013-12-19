// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: PaymentTypes.cs
// 
// Created by: ykors_000 at 19.12.2013 12:54
// 
// Property of SoftGears
// 
// ========
namespace LeadControl.Domain.Enums
{
    /// <summary>
    /// Типы оплаты
    /// </summary>
    public enum PaymentTypes: short
    {
        /// <summary>
        /// Оплата наличными
        /// </summary>
        [EnumDescription("Оплата наличными")]
        Cash = 1,

        /// <summary>
        /// Оплата системами интернет-эквайринга
        /// </summary>
        [EnumDescription("Интернет-эквайринг")]
        EPayments = 2,

        /// <summary>
        /// Наложенным платежем
        /// </summary>
        [EnumDescription("Наложенный платеж")]
        PostAttached = 3,

        /// <summary>
        /// Оплата банковским безналичным переводом по выставленному счету
        /// </summary>
        [EnumDescription("Банковский безналичный платеж")]
        BankPayment = 3,
    }
}