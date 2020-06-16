using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTableAssistant
{
    /// <summary>
    /// Схема таблиы
    /// </summary>
    public abstract class SchemasTable
    {
        /// <summary>
        /// Имя таблицы
        /// </summary>
        public string TableName { get; set; } = string.Empty;
        /// <summary>
        /// SQL запрос при первоначальном создании таблицы
        /// </summary>
        public string FirstQuery { get; set; } = string.Empty;
        /// <summary>
        /// SQL запрос выполняемый после обновления схемы таблицы
        /// </summary>
        public string UpdateQuery { get; set; } = string.Empty;
        /// <summary>
        /// Набор столбцов таблицы
        /// </summary>
        public List<SchemasColumn> Columns { get; private set; } = new List<SchemasColumn>();
        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        public SchemasTable() { }
        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        /// <param name="name">имя таблицы</param>
        public SchemasTable(string name)
        {
            TableName = name;
        }
        /// <summary>
        /// Устанавливает имя таблицы
        /// </summary>
        /// <param name="value"></param>
        public void SetTableName(string value)
        {
            TableName = value;
        }
        /// <summary>
        /// Добавляет новый столбец в схему таблицы
        /// </summary>
        /// <param name="name">имя столбца</param>
        /// <returns></returns>
        public SchemasColumn AddColumn(string name)
        {
            var col = new SchemasColumn(name);
            Columns.Add(col);
            return col;
        }
        /// <summary>
        /// Добавляет новый столбец в схему таблицы
        /// </summary>
        /// <param name="col">Объект схемы столбца</param>
        public void AddColumn(SchemasColumn col)
        {
            Columns.Add(col);
        }
        /// <summary>
        /// Задает SQL запрос выполняющийся при первом создании таблицы
        /// </summary>
        /// <param name="query"></param>
        public void SetFirstQuery(string query)
        {
            FirstQuery = query;
        }
        /// <summary>
        /// Задает SQL запрос, который выполниться в случае изменения схемы таблицы
        /// </summary>
        /// <param name="query"></param>
        public void SetUpdateQuery(string query)
        {
            UpdateQuery = query;
        }
    }
}
