namespace LeadControl.Domain.Interfaces.Cache
{
    /// <summary>
    /// Абстрактный интерфейс кеша строк
    /// </summary>
    public interface IStringCache
    {
        /// <summary>
        /// Пытается извлечь указанной значение из кеша строка
        /// </summary>
        /// <param name="key">Ключ кеша</param>
        /// <param name="value">переменная куда поместить значение если удасться извлечь</param>
        /// <returns>True если значение присутствует в кеше, иначе false</returns>
        bool TryGetFromCache(string key, out string value);

        /// <summary>
        /// Получает хранимое в кеше значение под указанным ключом
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns>Значение хранимое в кеше</returns>
        string Get(string key);

        /// <summary>
        /// Устанавливает значение в кеш под указанным ключом
        /// </summary>
        /// <param name="key">ключ</param>
        /// <param name="value">значение</param>
        void Set(string key, string value);

        /// <summary>
        /// Удаляет указанный ключ из кеша
        /// </summary>
        /// <param name="keyName">Ключ</param>
        void RemoveKey(string keyName);
    }
}