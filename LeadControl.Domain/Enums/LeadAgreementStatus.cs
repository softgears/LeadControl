// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: LeadAgreementStatus.cs
// 
// Created by: ykors_000 at 09.01.2014 12:00
// 
// Property of SoftGears
// 
// ========
namespace LeadControl.Domain.Enums
{
    /// <summary>
    /// Статусы договора
    /// </summary>
    public enum LeadAgreementStatus: short
    {
        /// <summary>
        /// Договор на стадии согласования
        /// </summary>
        [EnumDescription("Согласование")]
        Negotiation = 1,

        /// <summary>
        /// Договор на подписании
        /// </summary>
        [EnumDescription("На подписании")]
        OnSigning = 2,

        /// <summary>
        /// Договор подписан и работы по нему осуществляются
        /// </summary>
        [EnumDescription("Подписан")]
        Signed = 3,

        /// <summary>
        /// Договор истек
        /// </summary>
        [EnumDescription("Истек")]
        Expired = 4
    }
}