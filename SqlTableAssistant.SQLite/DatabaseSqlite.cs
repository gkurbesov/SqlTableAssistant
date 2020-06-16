using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlTableAssistant.Arguments;
using SqlTableAssistant.Interface;

namespace SqlTableAssistant.SQLite
{
    public class DatabaseSqlite : IMainDatabase
    {
        /// <summary>
        /// Событие об изменении состояния подключения к базе данных
        /// </summary>
        public event EventHandler<ConnectionArgs> OnConnectionStateChange;
        /// <summary>
        /// Событие об ошибках при работе с базой данных
        /// </summary>
        public event EventHandler<ErrorArgs> OnDatabaseError;
        /// <summary>
        /// Настройки подключения к базе данных
        /// </summary>
        public ISqlConfig config { get; private set; } = null;
        /// <summary>
        /// Экземпляр подключения к базе данных
        /// </summary>
        public SQLiteConnection connection { get; private set; } = null;
        /// <summary>
        /// Схема таблиц базы данных
        /// </summary>
        internal IEnumerable<SchemasTable> tables_schemas { get; set; } = Enumerable.Empty<SchemasTable>();
        private object locker = new object();
        /// <summary>
        /// Сообщить об изменении состояния подключения к базе данных
        /// </summary>
        /// <param name="status">статус подключения к БД</param>
        public void CallConnectionStatus(StatusDataBase status)
        {
            Task.Run(() =>
            {
                OnConnectionStateChange?.Invoke(this, new ConnectionArgs(config, status));
            });
        }
        /// <summary>
        /// Сообщить о возникновении ошибки при работе с базой данных
        /// </summary>
        /// <param name="msg">сообщение об ошибке</param>
        /// <param name="trace">стек вызова ошибки</param>
        public void CallError(string msg, string trace = "")
        {
            Task.Run(() =>
            {
                OnDatabaseError?.Invoke(this, new ErrorArgs(msg, trace));
            });
        }
        /// <summary>
        /// Получить новый экзепляр подключения к БД с текущими настройками
        /// </summary>
        /// <param name="open">предварительно открыть соединение</param>
        /// <returns></returns>
        public IDbConnection GetNewConnection(bool open)
        {
            try
            {
                var connection = new SQLiteConnection(config.GetConnectionString());
                if (open) connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                CallError(ex.Message, ex.StackTrace);
                return null;
            }
        }
        /// <summary>
        /// Установить новые настройки подключения
        /// </summary>
        /// <param name="value"></param>
        public void SetConfig(ISqlConfig value)
        {
            config = value;
            if (IsConnected()) Connect();
        }
        /// <summary>
        /// Проверяет соединение с базой данных
        /// </summary>
        /// <returns></returns>
        public bool IsConnected() => connection != null && connection.State == ConnectionState.Open;
        /// <summary>
        /// Подключиться к базе данных
        /// </summary>
        public void Connect()
        {
            Disconnect();
            CallConnectionStatus(StatusDataBase.Connecting);
            lock (locker)
            {
                connection = new SQLiteConnection(config.GetConnectionString());
                try
                {
                    connection.Open();
                    CallConnectionStatus(StatusDataBase.Connect);
                }
                catch (SQLiteException ex)
                {
                    CallError(ex.Message, ex.StackTrace);
                    CallConnectionStatus(StatusDataBase.ErrorConnecting);
                }
            }
        }
        /// <summary>
        /// Отключиться от базы данных
        /// </summary>
        public void Disconnect()
        {
            lock (locker)
            {
                try
                {
                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                        connection = null;
                    }
                    CallConnectionStatus(StatusDataBase.Close);
                }
                catch (Exception ex)
                {
                    CallError(ex.Message, ex.StackTrace);
                    CallConnectionStatus(StatusDataBase.ErrorCloseConnecting);
                }
            }
        }
        /// <summary>
        /// Первоначальная инициализация базы данных
        /// </summary>
        /// <param name="value">Коллекция со схемами таблиц</param>
        public void InitTables(IEnumerable<SchemasTable> value)
        {
            tables_schemas = value;
            this.InitSchemas();
        }
        /// <summary>
        /// Выполнить запрос вставки
        /// </summary>
        /// <param name="table_name">имя таблицы</param>
        /// <param name="query_params">словарь параметр:значение</param>
        /// <returns></returns>
        public long ExecuteInsert(string table_name, Dictionary<string, object> query_params)
        {
            if (IsConnected())
            {
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    try
                    {
                        string query = this.InsertString(table_name, query_params);

                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        foreach (var param in query_params)
                            command.Parameters.Add(new SQLiteParameter($"@{param.Key}", param.Value));
                        command.ExecuteNonQuery();
                        command.CommandText = @"select last_insert_rowid()";
                        var lastId = command.ExecuteScalar();
                        return long.TryParse(lastId.ToString(), out var id) ? id : -1;
                    }
                    catch (Exception ex)
                    {
                        CallError(ex.Message, ex.StackTrace);
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// Выполнить SQL без результата
        /// </summary>
        /// <param name="query">строка SQL запроса</param>
        /// <returns></returns>
        public bool ExecuteNonQuery(string query)
        {
            if (IsConnected())
            {
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        CallError(ex.Message, ex.StackTrace);
                    }
                }

            }
            return false;
        }
        /// <summary>
        /// Выполнить SQL без результата
        /// </summary>
        /// <param name="query">SQL запрос для выполнения с параметрами @param</param>
        /// <param name="query_params">словарь параметр:значение</param>
        /// <returns></returns>
        public bool ExecuteNonQuery(string query, Dictionary<string, object> query_params)
        {
            if (IsConnected())
            {
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        foreach (var param in query_params)
                            command.Parameters.Add(new SQLiteParameter($"@{param.Key}", param.Value));
                        command.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        CallError(ex.Message, ex.StackTrace);
                    }
                }

            }
            return false;
        }
        /// <summary>
        /// Выполнить скалярный SQL запрос
        /// </summary>
        /// <param name="query">строка SQL запроса</param>
        /// <returns></returns>
        public object ExecuteScalar(string query)
        {
            if (IsConnected())
            {
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = query;
                        return command.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        CallError(ex.Message, ex.StackTrace);
                    }
                }

            }
            return null;
        }
        /// <summary>
        /// Выполнить скалярный SQL запрос
        /// </summary>
        /// <param name="query">строка SQL запроса с параметрами @param</param>
        /// <param name="query_params">словарь параметр:значение</param>
        /// <returns></returns>
        public object ExecuteScalar(string query, Dictionary<string, object> query_params)
        {
            if (IsConnected())
            {
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        foreach (var param in query_params)
                            command.Parameters.Add(new SQLiteParameter($"@{param.Key}", param.Value));
                        return command.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        CallError(ex.Message, ex.StackTrace);
                    }
                }

            }
            return null;
        }

        public DataSet ExecuteSelect(string query)
        {
            if (IsConnected())
            {
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = query;
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);

                        DataSet data = new DataSet();
                        adapter.Fill(data);
                        return data;
                    }
                    catch (Exception ex)
                    {
                        CallError(ex.Message, ex.StackTrace);
                    }
                }
            }
            return null;
        }

        public DataSet ExecuteSelect(string query, Dictionary<string, object> query_params)
        {
            if (IsConnected())
            {
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        foreach (var param in query_params)
                            command.Parameters.Add(new SQLiteParameter($"@{param.Key}", param.Value));

                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);

                        DataSet data = new DataSet();
                        adapter.Fill(data);
                        return data;
                    }
                    catch (Exception ex)
                    {
                        CallError(ex.Message, ex.StackTrace);
                    }
                }
            }
            return null;
        }
    }
}
