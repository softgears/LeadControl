using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;

namespace LeadControl.Domain.IoC
{
    /// <remarks>
    /// В качестве IoC контейнера используется AutoFac
    /// </remarks>
    /// <summary>
    /// IoC локатор сервисов, предназначенный для резолвинга зависимостей с помощью IoC контейнера
    /// </summary>
    public static class Locator
    {
        /// <summary>
        /// IoC контейнер
        /// </summary>
        public static IContainer Container { get; private set; }

        /// <summary>
        /// Инициализирует IoC контейнер указанными модулями
        /// </summary>
        /// <param name="modules"></param>
        public static void Init(params IModule[] modules)
        {
            var builder = new ContainerBuilder();
            foreach (var module in modules)
            {
                builder.RegisterModule(module);
            }
            Container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
        }

        /// <summary>
        /// Получает реализацию или инстант типа с помощью IoC контейнера
        /// </summary>
        /// <typeparam name="T">Необходимый тип</typeparam>
        /// <returns>Реализация или инстанс указанного типа</returns>
        public static T GetService<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }

        /// <summary>
        /// Установка контейнера проинициализированного где то еще
        /// </summary>
        /// <param name="kernel"></param>
        public static void SetKernel(IContainer kernel)
        {
            Container = kernel;
        }

        /// <summary>
        /// Начинает вложенный скоуп жизни для зависимостей на HttpRequest
        /// </summary>
        /// <returns></returns>
        public static ILifetimeScope BeginNestedHttpRequestScope()
        {
            if (HttpContext.Current == null)
            {
                HttpContext.Current = new HttpContext(new HttpRequest("index.html","http://localhost",string.Empty),new HttpResponse(null));
            }
            return Container.BeginLifetimeScope("httpRequest");
        }
    }
}