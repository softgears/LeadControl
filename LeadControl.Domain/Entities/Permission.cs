// 
// 
// Solution: PartDesk
// Project: PartDesk.Domain
// File: Permission.cs
// 
// Created by: ykors_000 at 18.11.2013 13:23
// 
// Property of SoftGears
// 
// ========
namespace LeadControl.Domain.Entities
{
    /// <summary>
    /// Разрешение на выполнение определенного действия или группы действий или доступа к указанному разделу
    /// </summary>
    public partial class Permission
    {
        /// <summary>
        /// Управление ролями
        /// </summary>
        public const long ManageRoles = 1;

        /// <summary>
        /// Управление проектами
        /// </summary>
        public const long ManageProjects = 2;

        /// <summary>
        /// Управление пользователями
        /// </summary>
        public const long ManageUsers = 3;

        /// <summary>
        /// Управление товарами
        /// </summary>
        public const long ManageProducts = 4;

        /// <summary>
        /// Управление складами
        /// </summary>
        public const long ManageWarehouses = 5;

        /// <summary>
        /// Привилегии менеджера по заказам
        /// </summary>
        public const long OrdersManager = 6;

        /// <summary>
        /// Привилегии бухгалтера
        /// </summary>
        public const long Finances = 7;

        /// <summary>
        /// Привилегии логиста
        /// </summary>
        public const long Logistics = 8;

        /// <summary>
        /// Доступ к ВЭД заявкам
        /// </summary>
        public const long FEA = 9;

        /// <summary>
        /// Управление остатками на складах
        /// </summary>
        public const long Warehousing = 10;

        /// <summary>
        /// Привилегии менеджера по услугам
        /// </summary>
        public const long ServicesManager = 11;

        /// <summary>
        /// Управление услугами
        /// </summary>
        public const long ManageServices = 12;
    }
}