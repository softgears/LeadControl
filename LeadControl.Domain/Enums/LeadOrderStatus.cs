// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: LeadOrderStatus.cs
// 
// Created by: ykors_000 at 05.12.2013 15:10
// 
// Property of SoftGears
// 
// ========
namespace LeadControl.Domain.Enums
{
    /// <summary>
    /// Статусы заказа
    /// </summary>
    public enum LeadOrderStatus: short
    {
        /// <summary>
        /// Заказ введен в систему
        /// </summary>
        [EnumDescription("Создан")]
        Initial = 1,

        /// <summary>
        /// Заказ оплачен
        /// </summary>
        [EnumDescription("Оплачен")]
        Payed = 250,

        /// <summary>
        /// Заказ завершен
        /// </summary>
        [EnumDescription("Завершен")]
        Completed = 300
    }
}