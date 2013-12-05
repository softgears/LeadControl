// 
// 
// Solution: LeadControl
// Project: LeadControl.Web
// File: IoCConfig.cs
// 
// Created by: ykors_000 at 05.12.2013 11:09
// 
// Property of SoftGears
// 
// ========

using LeadControl.Domain.DAL;
using LeadControl.Domain.Infrastructure;
using LeadControl.Domain.IoC;
using LeadControl.Web.Classes;

namespace LeadControl.Web
{
    /// <summary>
    /// Конфигуратор IoC контейнера
    /// </summary>
    public static class IoCConfig
    {
        /// <summary>
        /// Инициализирует IoC контейнер для системы
        /// </summary>
        public static void Init()
        {
            Locator.Init(new DataAccessLayer(), new InfrasstructureLayer(), new WebLayer());
        }
    }
}