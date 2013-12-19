// 
// 
// Solution: LeadControl
// Project: LeadControl.Web
// File: LeadOrderFilterModel.cs
// 
// Created by: ykors_000 at 19.12.2013 13:19
// 
// Property of SoftGears
// 
// ========
namespace LeadControl.Web.Models.Orders
{
    /// <summary>
    /// Модель для построения фильтра заявок
    /// </summary>
    public class LeadOrderFilterModel
    {
        /// <summary>
        /// Адрес куда сабмитить форму фильтра
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// Модель используемая для выборки фильтра
        /// </summary>
        public OrdersFiltrationModel FilterData { get; set; }
    }
}