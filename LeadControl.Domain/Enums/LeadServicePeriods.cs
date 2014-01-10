// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: LeadServicePeriods.cs
// 
// Created by: ykors_000 at 09.01.2014 12:05
// 
// Property of SoftGears
// 
// ========
namespace LeadControl.Domain.Enums
{
    /// <summary>
    /// Периоды оказания услуг
    /// </summary>
    public enum LeadServicePeriods: short
    {
        /// <summary>
        /// Без периодичности
        /// </summary>
        [EnumDescription("Разово")]
        No = 0,

        /// <summary>
        /// Ежедневно
        /// </summary>
        [EnumDescription("Ежедневно")]
        Daily = 1,

        /// <summary>
        /// Еженедельно
        /// </summary>
        [EnumDescription("Еженедельно")]
        Weekly = 2,

        /// <summary>
        /// Ежемесячно
        /// </summary>
        [EnumDescription("Ежемесячно")]
        Montly = 3,

        /// <summary>
        /// Ежеквартально
        /// </summary>
        [EnumDescription("Ежеквартально")]
        Quartal = 4,

        /// <summary>
        /// Пол года
        /// </summary>
        [EnumDescription("Каждые пол года")]
        HalfYearly = 5,

        /// <summary>
        /// Раз в год
        /// </summary>
        [EnumDescription("Ежегодно")]
        Yearly = 6,
    }
}