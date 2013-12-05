// ============================================================
// 
// 	Astramed
// 	SoftGears.CMS.Web 
// 	ChangePasswordModel.cs
// 
// 	Created by: ykorshev 
// 	 at 21.06.2013 11:46
// 
// ============================================================
namespace LeadControl.Web.Models.Account
{
    /// <summary>
    /// МОдель изменения пароля на сайте
    /// </summary>
    public class ChangePasswordModel
    {
        /// <summary>
        /// СТарый пароль
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// Новый пароль
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Новый пароль еще раз
        /// </summary>
        public string NewPasswordConfirm { get; set; }
    }
}