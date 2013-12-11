// 
// 
// Solution: PartDesk
// Project: PartDesk.Domain
// File: User.cs
// 
// Created by: ykors_000 at 18.11.2013 11:40
// 
// Property of SoftGears
// 
// ========

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeadControl.Domain.Entities;
using LeadControl.Domain.Misc;

namespace LeadControl.Domain.Entities
{
    /// <summary>
    /// Авторизованный пользователь системы
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// Проверяет, есть ли у пользователя указанное разрешение
        /// </summary>
        /// <param name="requiredPermission"></param>
        /// <returns></returns>
        public bool HasPermission(long requiredPermission)
        {
            return Role.RolePermissions.Any(rp => rp.PermissionId == requiredPermission);
        }

        /// <summary>
        /// Возвращает количество заказов
        /// </summary>
        /// <returns></returns>
        public int GetOrdersCount()
        {
            return 0;
        }

        /// <summary>
        /// Возвращает урл аватарки
        /// </summary>
        /// <returns></returns>
        public string GetAvatar()
        {
            return String.Format("http://www.gravatar.com/avatar/{0}?d=monsterid",
                                 PasswordUtils.GenerateMD5PasswordHash(Email.Trim().ToLower()).ToLower());
        }

        /// <summary>
        /// Возвращает фио пользователя
        /// </summary>
        /// <returns></returns>
        public string GetFio()
        {
            return String.Format("{0} {1} {2}", LastName, FirstName, SurName).Trim();
        }

        /// <summary>
        /// Возвращает ФИО в сокращенном формате
        /// </summary>
        /// <returns></returns>
        public string GetFioSmall()
        {
            var sb = new StringBuilder();
            sb.Append(LastName);
            if (!String.IsNullOrEmpty(FirstName))
                sb.AppendFormat(" {0}.", FirstName.First());
            if (!String.IsNullOrEmpty(SurName))
                sb.AppendFormat(" {0}.", SurName.First());
            return sb.ToString();
        }

        /// <summary>
        /// Возвращает фио в международном формате
        /// </summary>
        /// <returns></returns>
        public string GetFioInternation()
        {
            var sb = new StringBuilder();
            sb.Append(FirstName);
            if (!String.IsNullOrEmpty(SurName))
                sb.AppendFormat(" {0}", SurName.First());
            if (!String.IsNullOrEmpty(LastName))
                sb.AppendFormat(" {0}", LastName);
            return sb.ToString().Transliterate();
        }

        /// <summary>
        /// Проверяет есть ли у пользователя административные разрешения
        /// </summary>
        /// <returns></returns>
        public bool HasAdministrativePermission()
        {
            return Role.RolePermissions.Any(p => p.Permission.Group == "Администрирование");
        }

        /// <summary>
        /// Яврляется ли пользователь администратором
        /// </summary>
        /// <returns></returns>
        public bool IsAdmin()
        {
            return Role.Id == 1;
        }
    }
}