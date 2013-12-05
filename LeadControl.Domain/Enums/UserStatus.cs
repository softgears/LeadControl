namespace LeadControl.Domain.Enums
{
    /// <summary>
    /// Статусы пользователя
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// Пользователь активен
        /// </summary>
        [EnumDescription("Активен")]
        Active = 1,

        /// <summary>
        /// Пользователь неактивен
        /// </summary>
        [EnumDescription("Неактивен")]
        Inactive = 2,

        /// <summary>
        /// Пользователь заблокирован
        /// </summary>
        [EnumDescription("Заблокирован")]
        Blocked = 3 
    }
}