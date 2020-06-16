using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTableAssistant.SQLite
{
    internal static class DatabaseSqliteExt
    {
        /// <summary>
        /// Инициализация таблиц базы данных
        /// </summary>
        /// <param name="db"></param>
        internal static void InitSchemas(this DatabaseSqlite db)
        {
            if (!db.IsConnected()) db.Connect();
            foreach (var table in db.tables_schemas)
            {
                bool isupdate = false;
                if (CheckTable(db, table.TableName))
                {
                    IEnumerable<SchemasColumn> dif_columns = new SchemasColumn[0];
                    if (!TryCheckColumns(db, table.TableName, table.Columns, out dif_columns))
                    {
                        foreach (var column in dif_columns)
                            CreatColumn(db, table.TableName, column);
                        isupdate = true;
                    }
                    if (isupdate && !string.IsNullOrWhiteSpace(table.UpdateQuery))
                        db.ExecuteNonQuery(table.UpdateQuery);
                }
                else
                {
                    CreatTable(db, table);
                }
            }
        }
        /// <summary>
        /// Проверка существования таблицы
        /// </summary>
        /// <param name="db"></param>
        /// <param name="table_name"></param>
        /// <returns></returns>
        internal static bool CheckTable(this DatabaseSqlite db, string table_name)
        {
            if (!db.IsConnected()) db.Connect();
            if (db.IsConnected())
            {
                string query = "SELECT name FROM sqlite_master WHERE type='table' AND name = @table_name;";
                Dictionary<string, object> param = new Dictionary<string, object>()
                {
                    { "table_name", table_name}
                };
                string tbl_name = db.ExecuteScalar(query, param)?.ToString();
                return !string.IsNullOrWhiteSpace(tbl_name);
            }
            return false;
        }
        /// <summary>
        /// Проверка существования столбца
        /// </summary>
        /// <param name="db"></param>
        /// <param name="table_name"></param>
        /// <param name="column_name"></param>
        /// <returns></returns>
        internal static bool CheckColumn(this DatabaseSqlite db, string table_name, string column_name)
        {
            if (!db.IsConnected()) db.Connect();
            if (db.IsConnected())
            {
                using (var command = db.connection.CreateCommand())
                {
                    try
                    {
                        string query = $"PRAGMA table_info({table_name});";
                        command.CommandText = query;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var name = reader["name"]?.ToString() ?? string.Empty;
                                if (name.Equals(column_name)) return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        db.CallError(ex.Message, ex.StackTrace);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Проверка существования столбцов
        /// </summary>
        /// <param name="db"></param>
        /// <param name="table_name"></param>
        /// <param name="columns"></param>
        /// <param name="dif_columns">коллекция столбцов, который отсутствуют в таблице</param>
        /// <returns>true - если таблица соответствует схеме</returns>
        internal static bool TryCheckColumns(this DatabaseSqlite db, string table_name, IEnumerable<SchemasColumn> columns, out IEnumerable<SchemasColumn> dif_columns)
        {
            if (!db.IsConnected()) db.Connect();
            if (db.IsConnected())
            {
                using (var command = db.connection.CreateCommand())
                {
                    try
                    {
                        string query = $"PRAGMA table_info({table_name});";
                        command.CommandText = query;
                        using (var reader = command.ExecuteReader())
                        {
                            List<string> list_columns = new List<string>();
                            while (reader.Read())
                            {
                                var name = reader["name"]?.ToString() ?? string.Empty;
                                if (!string.IsNullOrWhiteSpace(name)) list_columns.Add(name);
                            }
                            dif_columns = columns.Where(o => list_columns.Find(x => x.Equals(o.Name)) == null).ToArray();
                            return dif_columns.Count() == 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        db.CallError(ex.Message, ex.StackTrace);
                    }
                }
            }
            dif_columns = Enumerable.Empty<SchemasColumn>();
            return false;
        }
        /// <summary>
        /// Создание таблицы
        /// </summary>
        /// <param name="db"></param>
        /// <param name="structure"></param>
        /// <returns></returns>
        internal static bool CreatTable(this DatabaseSqlite db, SchemasTable structure)
        {
            if (!db.IsConnected()) db.Connect();
            if (db.IsConnected())
            {
                string columns = string.Join(", ", structure.Columns.Select(o => o.GetQueryString()));
                string query = $"CREATE TABLE {structure.TableName} ({columns});";
                if (db.ExecuteNonQuery(query))
                {
                    if (!string.IsNullOrWhiteSpace(structure.FirstQuery))
                        db.ExecuteNonQuery(structure.FirstQuery);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Создание столбца
        /// </summary>
        /// <param name="db"></param>
        /// <param name="table_name"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        internal static bool CreatColumn(this DatabaseSqlite db, string table_name, SchemasColumn column)
        {
            if (!db.IsConnected()) db.Connect();
            if (db.IsConnected())
            {
                string query = $"ALTER TABLE {table_name} ADD COLUMN {column.Name} {column.ColumnType}";
                if (column.DefaultContent != null)
                    query += $" DEFAULT ('{column.DefaultContent.ToString().Replace("'", "").Replace("\\", "")}')";
                return db.ExecuteNonQuery(query);
            }
            return false;
        }
    }
}
