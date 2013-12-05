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
        /// Поиск автозапчастей по артикулу
        /// </summary>
         public const long SearchParts = 1;

        /// <summary>
        /// Формирование заказа
        /// </summary>
        public const long MakeOrder = 2;

        /// <summary>
        /// Оплата заказа
        /// </summary>
        public const long PayOrder = 3;

        /// <summary>
        /// Отмена заказа
        /// </summary>
        public const long CancelOrder = 4;

        /// <summary>
        /// Доступ к разделу технической поддержки
        /// </summary>
        public const long SupportAccess = 5;

        /// <summary>
        /// Доступ к системе онлайн консультирования
        /// </summary>
        public const long OnlineConsultantAccess = 6;

        /// <summary>
        /// Управление компаниями
        /// </summary>
        public const long ManageCompanies = 7;

        /// <summary>
        /// Управление пользователями
        /// </summary>
        public const long ManageUsers = 8;

        /// <summary>
        /// Управление заказами
        /// </summary>
        public const long ManageOrders = 9;

        /// <summary>
        /// Управление ролями
        /// </summary>
        public const long ManageRoles = 10;

        /// <summary>
        /// Управление разделом технической поддержки
        /// </summary>
        public const long ManageSupport = 11;

        /// <summary>
        /// Управление настройками работы системы
        /// </summary>
        public const long ManageSettings = 12;

        /// <summary>
        /// Просмотр событий аудита системы
        /// </summary>
        public const long ManageAudit = 13;
    }
}