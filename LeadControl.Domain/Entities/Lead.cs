// 
// 
// Solution: LeadControl
// Project: LeadControl.Domain
// File: Lead.cs
// 
// Created by: ykors_000 at 09.12.2013 12:31
// 
// Property of SoftGears
// 
// ========

using System;
using System.Text;

namespace LeadControl.Domain.Entities
{
    /// <summary>
    /// Клиент системы
    /// </summary>
    public partial class Lead
    {
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (LeadLegalInfos != null && LeadLegalInfos.LegalType != 0)
            {
                if (!String.IsNullOrEmpty(LeadLegalInfos.CompanyName))
                {
                    sb.Append(LeadLegalInfos.CompanyName);
                }
                else
                {
                    sb.AppendFormat("{0} {1} {2}", LastName, FirstName, SurName);
                }
            }
            else
            {
                sb.AppendFormat("{0} {1} {2}", LastName, FirstName, SurName);
            }
            if (sb.ToString().Trim().Length == 0)
            {
                sb.Append("Нет данных по имени");
            }
            return sb.ToString();
        }
    }
}