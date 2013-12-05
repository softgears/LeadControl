using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace LeadControl.Domain.Routing
{
    /// <summary>
    /// Статический менеджер роутов
    /// </summary>
    public static class RoutesManager
    {
        /// <summary>
        /// Регистрирует указанный роут с указанными параметрами маппинга
        /// </summary>
        /// <param name="name"></param>
        /// <param name="route">Строковый путь роута</param>
        /// <param name="routeMapping">Маппинг</param>
        /// <param name="insertFirst">Вставить ли роут самым первым в таблице роутов</param>
        public static void RegisterRoute(string name, string route, object routeMapping, bool insertFirst = false)
        {
            var newRoute = RouteTable.Routes.MapRoute(name, route, routeMapping);
            if (insertFirst)
            {
                RouteTable.Routes.Remove(newRoute);
                RouteTable.Routes.Insert(0,newRoute);
            }
        }

        /// <summary>
        /// Удаляет указанный роут из таблицы роутов
        /// </summary>
        /// <param name="route">Роут</param>
        public static void RemoveRoute(string route)
        {
            var findedRoute = RouteTable.Routes.Cast<Route>().FirstOrDefault(r => r.Url.ToLower() == route.ToLower());
            if (findedRoute != null)
            {
                RouteTable.Routes.Remove(findedRoute);
            }
        }

        /// <summary>
        /// Обновляет указанный роут на новое значение
        /// </summary>
        /// <param name="oldRoute">старый роут</param>
        /// <param name="newRoute">новый роут</param>
        public static void UpdateRoute(string oldRoute, string newRoute)
        {
            var findedRoute = RouteTable.Routes.Cast<Route>().FirstOrDefault(r => r.Url.ToLower() == oldRoute.ToLower());
            if (findedRoute != null)
            {
                findedRoute.Url = newRoute;
            }
        }

        /// <summary>
        /// Вспомогательный метод, сканирующий все доступные сборки на предмет наличия у методов классов атрибутов RouteAttribute и устанавливающий указанный роут на них соответствующий метод класса
        /// </summary>
        public static void RegisterActionRoutes()
        {
            // Получаем информацию о контроллерах и действиях, помеченных атрибутом роутинга используя три вложенные монады
            var methodsMappings = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from type in assembly.GetTypes()
                                  from method in type.GetMethods()
                                  let attributes = method.GetCustomAttributes(typeof (RouteAttribute), false)
                                  where attributes.Length > 0
                                  select
                                      new
                                          {
                                              Controller = type.Name,
                                              Action = method.Name,
                                              Attribute = (RouteAttribute) attributes.First()
                                          };
            // Регистрируем роуты
            foreach (var info in methodsMappings)
            {
                RegisterRoute(String.Format("{0}.{1}",info.Controller,info.Action),info.Attribute.Route,new {controller = info.Controller.Replace("Controller",String.Empty), action = info.Action});
            }
        }
    }
}