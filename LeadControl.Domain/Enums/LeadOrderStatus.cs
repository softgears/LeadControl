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
        /// Заказ введен в систему
        /// </summary>
        [EnumDescription("Сформирован")]
        Collected = 2,

        /// <summary>
        /// Заказ введен в систему
        /// </summary>
        [EnumDescription("Подтвержден")]
        Confirmed = 3,

        /// <summary>
        /// Заказ введен в систему
        /// </summary>
        [EnumDescription("Ожидание оплаты")]
        WaitingPayment = 10,

        /// <summary>
        /// Заказ введен в систему
        /// </summary>
        [EnumDescription("Подготовка к отправке")]
        Preparing = 200,

        /// <summary>
        /// Заказ введен в систему
        /// </summary>
        [EnumDescription("Отправлен")]
        Sent = 250,

        /// <summary>
        /// Заказ завершен
        /// </summary>
        [EnumDescription("Завершен")]
        Completed = 300
    }
}