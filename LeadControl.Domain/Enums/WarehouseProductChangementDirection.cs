// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: WarehouseProductChangementDirection.cs
// 
// Created by: ykors_000 at 12.12.2013 12:19
// 
// Property of SoftGears
// 
// ========
namespace LeadControl.Domain.Enums
{
    /// <summary>
    /// Операция с остатками на складе
    /// </summary>
    public enum WarehouseProductChangementDirection: short
    {
        /// <summary>
        /// Поступление на склад
        /// </summary>
        [EnumDescription("Поступление")]
        Income = 0,

        /// <summary>
        /// Изъятие со склада
        /// </summary>
        [EnumDescription("Изъятие")]
        Outcome = 1,
    }
}