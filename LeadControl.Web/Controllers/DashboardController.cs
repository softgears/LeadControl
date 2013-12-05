using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeadControl.Domain.Routing;
using LeadControl.Web.Classes.Security;
using LeadControl.Web.Models.Dashboard;

namespace LeadControl.Web.Controllers
{
    /// <summary>
    /// Контроллер управления сводкой
    /// </summary>
    public class DashboardController : BaseController
    {
        /// <summary>
        /// Отображает сводочную информацию, которая разнится в зависимости от роли пользователя
        /// </summary>
        /// <returns></returns>
        [AuthorizationCheck]
        public ActionResult Index()
        {
            PushNavigationItem("Дашбоард","/dashboard");

            return View(new DashboardModel());
        }

    }
}
