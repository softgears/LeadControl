namespace LeadControl.Web.Classes.Navigation
{
    /// <summary>
    /// Элемент навигационной цепочки
    /// </summary>
    public class NavigationChainItem
    {
        /// <summary>
        /// Заголовок ссылки
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Описание - всплывающая подсказка
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Адрес куда она ведет
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Элемент доступен для кликанья и перехода по ссылке
        /// </summary>
        public bool Active { get; set; }
    }
}