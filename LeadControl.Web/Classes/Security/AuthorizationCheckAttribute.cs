using System.Linq;
using System.Web.Mvc;
using LeadControl.Domain.DAL;
using LeadControl.Domain.Enums;
using LeadControl.Domain.Interfaces.Notifications;
using LeadControl.Domain.IoC;
using LeadControl.Web.Controllers;

namespace LeadControl.Web.Classes.Security
{
    /// <summary>
    /// Аспект, валидирующий авторизованность пользователя на выполнение действия
    /// </summary>
    public class AuthorizationCheckAttribute: ActionFilterAttribute
    {
        /// <summary>
        /// Урл, куда происходит редирект если пользователь не авторизован
        /// </summary>
        public string RedirectUrl { get; private set; }

        /// <summary>
        /// Дополнительный пермишен, которым должен обладать пользователь для успешного продолжения авторизация
        /// </summary>
        public long RequiredPermission { get; private set; }

        /// <summary>
        /// Инициализирует новый атрибут, помещающий действие аспектом
        /// </summary>
        /// <param name="requiredPermission">Дополнительное разрешение, которым должен обладать пользователь чтобы пройти авторизацию</param>
        /// <param name="redirectUrl">Урл, куда редиректить неавторизованного пользователя</param>
        public AuthorizationCheckAttribute(long requiredPermission = -1 ,string redirectUrl = "/account/login")
        {
            RequiredPermission = requiredPermission;
            RedirectUrl = redirectUrl;
        }

        /// <summary>
        /// Фильтруем действие перед его выполнение
        /// </summary>
        /// <param name="filterContext">Контекст действия</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var currentUser = (filterContext.Controller as BaseController).CurrentUser;
            if (currentUser == null)
            {
                filterContext.Result = new RedirectResult(this.RedirectUrl);
            } 
            else
            {
                var dc = Locator.GetService<LCDataContext>();
                // Проверка того, что пользователь забанен
                if (currentUser.Status == (int)UserStatus.Inactive)
                {
                    Locator.GetService<IUINotificationManager>().Error("Вы не активированы, поэтому не можете получить доступ к личному кабинету. Пожалуйста, свяжитесь с вашей компанией, и попросите уполномоченного сотрудника активировать вас.");
                    filterContext.Result = new RedirectResult("/");
                }
                else if (currentUser.Status == (int)UserStatus.Blocked)
                {
                    Locator.GetService<IUINotificationManager>().Error("Вы были заблокированы администратором, поэтому больше не имеете права пользоваться функциями системы");
                    filterContext.Result = new RedirectResult("/");
                } else if (RequiredPermission != -1 && !currentUser.HasPermission(RequiredPermission))
                {
                    // Проверка того что пользователь имеет требуемые разрешения
                    var permission = dc.Permissions.FirstOrDefault(p => p.Id == RequiredPermission);
                    Locator.GetService<IUINotificationManager>().Error(string.Format("Отсутствует необходимая привелегия ({0}) для доступа к указанному разделу", permission.Name));
                    filterContext.Result = new RedirectResult(this.RedirectUrl);    
                }
                else
                {
                    base.OnActionExecuting(filterContext);        
                }
            }
        }
    }
}