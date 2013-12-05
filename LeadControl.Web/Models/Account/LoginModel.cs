// ============================================================
// 
// 	Astramed
// 	SoftGears.CMS.Web 
// 	LoginModel.cs
// 
// 	Created by: ykorshev 
// 	 at 21.06.2013 10:32
// 
// ============================================================
namespace LeadControl.Web.Models.Account
{
    /// <summary>
    /// Модель авторизации на сайте
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Емейл
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Запомнить меня
        /// </summary>
        public bool RememberMe { get; set; }
    }
}