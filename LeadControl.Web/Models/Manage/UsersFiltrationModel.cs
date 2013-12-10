// 
// 
// Solution: LeadControl
// Project: LeadControl.Web
// File: UsersFiltrationModel.cs
// 
// Created by: ykors_000 at 10.12.2013 15:13
// 
// Property of SoftGears
// 
// ========

using System.Runtime.Remoting.Messaging;
using LeadControl.Domain.Entities;

namespace LeadControl.Web.Models.Manage
{
    /// <summary>
    /// Модель выборки пользователей
    /// </summary>
    public class UsersFiltrationModel: BaseFiltrationModel<User>
    {
        /// <summary>
        /// Идентификаторы ролей
        /// </summary>
        public long[] RoleIds { get; set; }

        public UsersFiltrationModel()
        {
            RoleIds = new long[]{};
        }
    }
}