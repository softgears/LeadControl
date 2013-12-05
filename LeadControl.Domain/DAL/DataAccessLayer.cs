// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: DataAccessLayer.cs
// 
// Created by: ykors_000 at 05.12.2013 10:59
// 
// Property of SoftGears
// 
// ========

using Autofac;
using Autofac.Integration.Mvc;

namespace LeadControl.Domain.DAL
{
    /// <summary>
    /// Модуль регистрации зависимостей DAL слоя
    /// </summary>
    public class DataAccessLayer: Autofac.Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        /// <param name="builder">The builder through which components can be
        ///             registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LCDataContext>().AsSelf().InstancePerHttpRequest();
        }
    }
}