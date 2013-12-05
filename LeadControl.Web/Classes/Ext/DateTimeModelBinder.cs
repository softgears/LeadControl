// ============================================================
// 
// 	Asgard
// 	Asgard.Web.Public 
// 	DateTimeModelBinder.cs
// 
// 	Created by: ykorshev 
// 	 at 04.08.2013 22:00
// 
// ============================================================

using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace LeadControl.Web.Classes.Ext
{
    public class DateTimeModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// Привязывает указанное свойство, используя заданные контекст контроллера и контекст привязки, а также заданный дескриптор свойства.
        /// </summary>
        /// <param name="controllerContext">Контекст, в котором функционирует контроллер.Сведения о контексте включают информацию о контроллере, HTTP-содержимом, контексте запроса и данных маршрута.</param><param name="bindingContext">Контекст, в котором привязана модель.Контекст содержит такие сведения, как объект модели, имя модели, тип модели, фильтр свойств и поставщик значений.</param><param name="propertyDescriptor">Описывает свойство, которое требуется привязать.Дескриптор предоставляет информацию, такую как тип компонента, тип свойства и значение свойства.Также предоставляет методы для получения или задания значения свойства.</param>
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.PropertyType == typeof(DateTime))
            {
                var submittedVal = controllerContext.HttpContext.Request[propertyDescriptor.Name];
                if (!String.IsNullOrEmpty(submittedVal))
                {
                    DateTime val = DateTime.MinValue;
                    try
                    {
                        val = Convert.ToDateTime(submittedVal);
                    }
                    catch (Exception)
                    {
                    }
                    propertyDescriptor.SetValue(bindingContext.Model, val);
                }
            }
            else if (propertyDescriptor.PropertyType == typeof(DateTime?))
            {
                var submittedVal = controllerContext.HttpContext.Request[propertyDescriptor.Name];
                if (!String.IsNullOrEmpty(submittedVal))
                {
                    DateTime? val = null;
                    try
                    {
                        val = Convert.ToDateTime(controllerContext.HttpContext.Request[propertyDescriptor.Name]);
                    }
                    catch (Exception)
                    {
                        val = null;
                    }
                    propertyDescriptor.SetValue(bindingContext.Model, val);
                }
            } else
            {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }
    }
}