using MySql.Data.MySqlClient;
using SqlTableAssistant.Arguments;
using SqlTableAssistant.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTableAssistant.MySql
{
    public class DatabaseMysql : IMainDatabase
    {
        public event EventHandler<ConnectionArgs> OnConnectionStateChange;
        public event EventHandler<ErrorArgs> OnDatabaseError;
        /// <summary>
        /// Настройки подключения к базе данных
        /// </summary>
        public ISqlConfig config { get; private set; } = null;
        /// <summary>
        /// Основной экземпляр подключения к базе данных
        /// </summary>
        public MySqlConnection connection { get; private set; } = null;
        /// <summary>
        /// Схема таблиц базы данных
        /// </summary>
        internal IEnumerable<SchemasTable> tables { get; set; } = Enumerable.Empty<SchemasTable>();
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
        public IDbConnection GetNewConnection(bool open = true)
        {
            try
            {
                var new_connection = new MySqlConnection(config.GetConnectionString());
                if (open) new_connection.Open();
                return new_connection;
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
                connection = new MySqlConnection(config.GetConnectionString());
                try
                {
                    connection.Open();
                    CallConnectionStatus(StatusDataBase.Connect);
                }
                catch (Exception ex)
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
            tables = value;
        }
        /// <summary>
        /// Выполнить запрос вставки
        /// </summary>
        /// <param name="table_name">имя таблицы</param>
        /// <param name="query_params">словарь параметр:значение</param>
        /// <returns></returns>
        public long ExecuteInsert(string table_name, Dictionary<string, object> query_params)
        {
            using (MySqlConnection connection_exp = GetNewConnection() as MySqlConnection)
            {
                try
                {
                    string query = this.InsertString(table_name, query_params);
                    using (MySqlCommand command = new MySqlCommand(query, connection_exp))
                    {
                        foreach (var param in query_params)
                            command.Parameters.Add(new MySqlParameter($"@{param.Key}", param.Value));
                        command.ExecuteNonQuery();
                        return command.LastInsertedId;
                    }
                }
                catch (Exception ex)
                {
                    CallError(ex.Message, ex.StackTrace);
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
            bool result = false;
            using (MySqlConnection connection_exp = GetNewConnection() as MySqlConnection)
            {
                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection_exp))
                    {
                        command.ExecuteNonQuery();
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    CallError(ex.Message, ex.StackTrace);
                }
            }
            return result;
        }
        /// <summary>
        /// Выполнить SQL без результата
        /// </summary>
        /// <param name="query">SQL запрос для выполнения с параметрами @param</param>
        /// <param name="query_params">словарь параметр:значение</param>
        /// <returns></returns>
        public bool ExecuteNonQuery(string query, Dictionary<string, object> query_params)
        {
            bool result = false;
            using (MySqlConnection connection_exp = GetNewConnection() as MySqlConnection)
            {
                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection_exp))
                    {
                        foreach (var param in query_params)
                            command.Parameters.Add(new MySqlParameter($"@{param.Key}", param.Value));
                        command.ExecuteNonQuery();
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    CallError(ex.Message, ex.StackTrace);
                }
            }
            return result;
        }
        /// <summary>
        /// Выполнить скалярный SQL запрос
        /// </summary>
        /// <param name="query">строка SQL запроса</param>
        /// <returns></returns>
        public object ExecuteScalar(string query)
        {
            object result = null;
            using (MySqlConnection connection_exp = GetNewConnection() as MySqlConnection)
            {
                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection_exp))
                    {
                        result = command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    CallError(ex.Message, ex.StackTrace);
                }
            }
            return result;
        }
        /// <summary>
        /// Выполнить скалярный SQL запрос
        /// </summary>
        /// <param name="query">строка SQL запроса с параметрами @param</param>
        /// <param name="query_params">словарь параметр:значение</param>
        /// <returns></returns>
        public object ExecuteScalar(string query, Dictionary<string, object> query_params)
        {
            object result = null;
            using (MySqlConnection connection_exp = (MySqlConnection)GetNewConnection())
            {
                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection_exp))
                    {
                        foreach (var param in query_params)
                            command.Parameters.Add(new MySqlParameter($"@{param.Key}", param.Value));
                        result = command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    CallError(ex.Message, ex.StackTrace);
                }
            }
            return result;
        }

        public DataSet ExecuteSelect(string query)
        {
            using (MySqlConnection connection_exp = GetNewConnection() as MySqlConnection)
            {
                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection_exp))
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        DataSet data = new DataSet();
                        adapter.Fill(data);
                        return data;
                    }
                }
                catch (Exception ex)
                {
                    CallError(ex.Message, ex.StackTrace);
                }
            }
            return null;
        }

        public DataSet ExecuteSelect(string query, Dictionary<string, object> query_params)
        {
            using (MySqlConnection connection_exp = GetNewConnection() as MySqlConnection)
            {
                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection_exp))
                    {
                        foreach (var param in query_params)
                            command.Parameters.Add(new MySqlParameter($"@{param.Key}", param.Value));
                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        DataSet data = new DataSet();
                        adapter.Fill(data);
                        return data;
                    }
                }
                catch (Exception ex)
                {
                    CallError(ex.Message, ex.StackTrace);
                }
            }
            return null;
        }
    }
}
