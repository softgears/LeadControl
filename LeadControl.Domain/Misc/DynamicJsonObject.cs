using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json.Linq;

namespace LeadControl.Domain.Misc
{
    /// <summary>
    /// Динамический Json объект
    /// </summary>
    public class DynamicJsonObject : DynamicObject
    {
        /// <summary>
        /// Коллекция свойств объекта
        /// </summary>
        private IDictionary<string, object> Properties { get; set; }

        /// <summary>
        /// JSON объект
        /// </summary>
        private JObject JsonObject { get; set; }

        /// <summary>
        /// Приватный конструктор используемый внутри
        /// </summary>
        /// <param name="jsonObject">Родительский Json объект</param>
        private DynamicJsonObject(JObject jsonObject)
            : this()
        {
            this.JsonObject = jsonObject;
            InitializeProperties();
        }

        /// <summary>
        /// Инициализирует Json объект заполняя его свойствами
        /// </summary>
        private void InitializeProperties()
        {
            if (JsonObject == null)
            {
                throw new Exception("JSON объект равен null, возможно неправильный JSON");
            }
            foreach (var property in JsonObject.Properties())
            {
                Properties[property.Name] = ProcessJValue(property.Value);
            }
        }

        /// <summary>
        /// Обрабатывает значение и возвращает подходящий результат
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object ProcessJValue(JToken value)
        {
            if (value is JObject)
            {
                return new DynamicJsonObject(value as JObject);
            }
            else if (value is JArray)
            {
                var array = new ArrayList();
                foreach (var arrayItem in (value as JArray))
                {
                    array.Add(ProcessJValue(arrayItem));
                }
                return array;
            }
            else
            {
                return value.Value<object>();
            }
        }

        /// <summary>
        /// Публичный конструктор на базе строк
        /// </summary>
        /// <param name="json">JSON в виде строке</param>
        public DynamicJsonObject(string json)
            : this(JObject.Parse(json))
        {

        }

        /// <summary>
        /// Базовый конструктор
        /// </summary>
        protected DynamicJsonObject()
        {
            this.Properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Пытается получить Json значение
        /// </summary>
        /// <param name="binder">Биндер</param>
        /// <param name="result">Результат</param>
        /// <returns>Можно получить или нет</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var found = Properties.TryGetValue(binder.Name, out result);
            if (!found)
            {
                result = null;
            }
            return true;
        }
    }

}