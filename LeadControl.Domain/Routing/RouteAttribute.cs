using System;

namespace LeadControl.Domain.Routing
{
    /// <remarks>
    /// Метод будет замаплен в таблицу роутов, если имеет тип результата ActionResult, иначе метод будет пропущен
    /// </remarks>
    /// <summary>
    /// Маркирует Action помеченным на указанный роут
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteAttribute: Attribute
    {
        /// <summary>
        /// Роут, на который указавает этот Action
        /// </summary>
        public string Route { get; private set; }

        /// <summary>
        /// Маркирует метод класса указанным роутом
        /// </summary>
        /// <param name="route"></param>
        public RouteAttribute(string route)
        {
            Route = route;
        }
    }
}