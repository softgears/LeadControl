// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: FEAOrderStatus.cs
// 
// Created by: ykors_000 at 09.12.2013 13:08
// 
// Property of SoftGears
// 
// ========
namespace LeadControl.Domain.Enums
{
    /// <summary>
    /// Статус ВЭД заявки
    /// </summary>
    public enum FEAOrderStatus: short
    {
        /// <summary>
        /// Заявка успешно выполнена
        /// </summary>
        [EnumDescription("Завершена")]
        Completed = 100
    }
}