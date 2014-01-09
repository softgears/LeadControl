// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: LeadServiceTypes.cs
// 
// Created by: ykors_000 at 09.01.2014 12:04
// 
// Property of SoftGears
// 
// ========
namespace LeadControl.Domain.Enums
{
    /// <summary>
    /// Типы услуг
    /// </summary>
    public enum LeadServiceTypes: short
    {
        /// <summary>
        /// Услуга оказывается разово
        /// </summary>
        [EnumDescription("Разования")]
        OneShot = 1,

        /// <summary>
        /// Услуга оказывается периодически
        /// </summary>
        [EnumDescription("Периодическая")]
        Periodical = 2
    }
}