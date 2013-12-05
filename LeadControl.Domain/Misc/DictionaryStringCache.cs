using System.Collections.Generic;
using LeadControl.Domain.Interfaces.Cache;

namespace LeadControl.Domain.Misc
{
    /// <summary>
    /// Реализация строкового кеша с помощью словаря - глобальная кеширующая таблица строк
    /// </summary>
    public class DictionaryStringCache: IStringCache
    {
        /// <summary>
        /// Сама кеширующая таблица
        /// </summary>
        private Dictionary<string, string> Cache { get; set; }

        /// <summary>
        /// Создает новый объект кеширующей таблицы
        /// </summary>
        public DictionaryStringCache()
        {
            Cache = new Dictionary<string, string>();
        }

        /// <summary>
        /// Пытается извлечь указанной значение из кеша строка
        /// </summary>
        /// <param name="key">Ключ кеша</param>
        /// <param name="value">переменная куда поместить значение если удасться извлечь</param>
        /// <returns>True если значение присутствует в кеше, иначе false</returns>
        public bool TryGetFromCache(string key, out string value)
        {
            return Cache.TryGetValue(key, out value);
        }

        /// <summary>
        /// Получает хранимое в кеше значение под указанным ключом
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns>Значение хранимое в кеше</returns>
        public string Get(string key)
        {
            return Cache[key];
        }

        /// <summary>
        /// Устанавливает значение в кеш под указанным ключом
        /// </summary>
        /// <param name="key">ключ</param>
        /// <param name="value">значение</param>
        public void Set(string key, string value)
        {
            // Блокируем коллекцию от одновременных изменений
            lock(Cache)
            {
                Cache[key] = value;
            }
        }

        /// <summary>
        /// Удаляет указанный ключ из кеша
        /// </summary>
        /// <param name="keyName">Ключ</param>
        public void RemoveKey(string keyName)
        {
            lock(Cache)
            {
                if (Cache.ContainsKey(keyName))
                {
                    Cache.Remove(keyName);
                }
            }
        }
    }
}