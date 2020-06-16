using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTableAssistant
{
    public class SchemasColumn
    {
        /// <summary>
        /// Имя столбца
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Тип данных столбца
        /// </summary>
        public string ColumnType { get; set; } = string.Empty;
        /// <summary>
        /// Флаг указывающий является ли столбец ключом
        /// </summary>
        public bool IsPrimaryKey { get; set; } = false;
        /// <summary>
        /// Содержимое по умолчанию
        /// </summary>
        public object DefaultContent { get; set; } = null;
        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        public SchemasColumn() { }
        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        /// <param name="name">имя столбца</param>
        public SchemasColumn(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Установить имя столбца
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SchemasColumn SetName(string value)
        {
            Name = value;
            return this;
        }
        /// <summary>
        /// Установить тип столбца
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SchemasColumn SetType(string value)
        {
            ColumnType = value;
            return this;
        }
        /// <summary>
        /// Установить флаг ключа
        /// </summary>
        /// <returns></returns>
        public SchemasColumn IsKey()
        {
            IsPrimaryKey = true;
            return this;
        }
        /// <summary>
        /// Установить содержимое по умолчанию
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SchemasColumn SetDefault(object value)
        {
            DefaultContent = value;
            return this;
        }
        /// <summary>
        /// Получить текст для SQL запрос ана создание столбца
        /// </summary>
        /// <returns></returns>
        public string GetQueryString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(" ");
            sb.Append(ColumnType.ToString());
            if (IsPrimaryKey)
            {
                sb.Append(" PRIMARY KEY AUTOINCREMENT");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(DefaultContent?.ToString()))
                {
                    sb.Append(" DEFAULT ('");
                    sb.Append(DefaultContent.ToString().Replace("'", "").Replace("\\", ""));
                    sb.Append("')");
                }
            }
            return sb.ToString();
        }
    }
}
