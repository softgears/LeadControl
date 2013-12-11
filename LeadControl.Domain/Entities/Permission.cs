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
        /// Доступ к ВЭД заявкам
        /// </summary>
        public const long FEA = 9;
    }
}