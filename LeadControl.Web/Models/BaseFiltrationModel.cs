// 
// 
// Solution: LeadControl
// Project: LeadControl.Web
// File: BaseFiltrationModel.cs
// 
// Created by: ykors_000 at 09.12.2013 11:05
// 
// Property of SoftGears
// 
// ========

using System.Collections;
using System.Collections.Generic;

namespace LeadControl.Web.Models
{
    /// <summary>
    /// Базовая модель для фильтрации
    /// </summary>
    public class BaseFiltrationModel<T> where T: class
    {
        /// <summary>
        /// Строка для фильтрации
        /// </summary>
        public string Term { get; set; }

        /// <summary>
        /// Текущая страница
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Всего элементов
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Выбранные элементы
        /// </summary>
        public IList<T> Fetched { get; set; }
    }
}