// 
// 
// Solution: PartDesk
// Project: PartDesk.Domain
// File: Role.cs
// 
// Created by: ykors_000 at 20.11.2013 12:55
// 
// Property of SoftGears
// 
// ========

using System.Linq;

namespace LeadControl.Domain.Entities
{
    /// <summary>
    /// Роль пользователей в системе
    /// </summary>
    public partial class Role
    {
        /// <summary>
        /// Возвращает пользователей, которые в этой роли находятся
        /// </summary>
        /// <returns></returns>
        public int GetUsersCount()
        {
            return Users.Count;
        }

        /// <summary>
        /// Проверяет, есть ли у роли указанное разрешение
        /// </summary>
        /// <param name="permissionId">Идентификатор разрешения</param>
        /// <returns></returns>
        public bool HasPermission(long permissionId)
        {
            return RolePermissions.Any(p => p.PermissionId == permissionId);
        }
    }
}