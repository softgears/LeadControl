// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: InfrasstructureLayer.cs
// 
// Created by: ykors_000 at 05.12.2013 10:57
// 
// Property of SoftGears
// 
// ========

using Autofac;
using LeadControl.Domain.Interfaces.Cache;
using LeadControl.Domain.Interfaces.Notifications;
using LeadControl.Domain.Misc;
using LeadControl.Domain.Notifications.Mailing;
using LeadControl.Domain.Notifications.SMS;

namespace LeadControl.Domain.Infrastructure
{
    /// <summary>
    /// Модуль регистрации зависимостей инфраструктурного слоя
    /// </summary>
    public class InfrasstructureLayer: Autofac.Module
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
            builder.RegisterType<UniSenderMailNotificationManager>().As<IMailNotificationManager>().SingleInstance();
            builder.RegisterType<SMSNotificationManager>().As<ISMSNotificationManager>().SingleInstance();
            builder.RegisterType<DictionaryStringCache>().As<IStringCache>().SingleInstance();
        }
    }
}