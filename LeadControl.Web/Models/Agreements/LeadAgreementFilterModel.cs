// 
// 
// Solution: LeadControl
// Project: LeadControl.Web
// File: LeadAgreementFilterModel.cs
// 
// Created by: ykors_000 at 09.01.2014 15:58
// 
// Property of SoftGears
// 
// ========
namespace LeadControl.Web.Models.Agreements
{
    /// <summary>
    /// Модель для построения фильтра договоров
    /// </summary>
    public class LeadAgreementFilterModel
    {
        /// <summary>
        /// УРЛ куда отправляется каллбак
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// Данные фильтра
        /// </summary>
        public AgreementsFiltrationModel FilterData { get; set; }
    }
}