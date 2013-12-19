// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: DeliveryTypes.cs
// 
// Created by: ykors_000 at 19.12.2013 12:52
// 
// Property of SoftGears
// 
// ========
namespace LeadControl.Domain.Enums
{
    /// <summary>
    /// Типы доставки
    /// </summary>
    public enum DeliveryTypes: short
    {
        /// <summary>
        /// Самовывоз
        /// </summary>
        [EnumDescription("Самовывоз")]
        Self = 1,

        /// <summary>
        /// Самовывоз
        /// </summary>
        [EnumDescription("Почта России")]
        RussianPost = 2,

        /// <summary>
        /// Курьерская служба
        /// </summary>
        [EnumDescription("Курьерской службой")]
        Courier = 3,

        /// <summary>
        /// Логистическая компания
        /// </summary>
        [EnumDescription("Логистическая компания")]
        LogisticsCompany = 4,
    }
}