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
        /// Заявка в стадии формирование
        /// </summary>
        [EnumDescription("Формирование")]
        Gathering = 1,

        /// <summary>
        /// Заявка успешно сформирована
        /// </summary>
        [EnumDescription("Сформирована")]
        Gathered = 2,

        /// <summary>
        /// Заявка в процессе выполнения
        /// </summary>
        [EnumDescription("В процессе выполнения")]
        InProcess = 3,

        /// <summary>
        /// Заявка успешно выполнена
        /// </summary>
        [EnumDescription("Завершена")]
        Completed = 100
    }
}