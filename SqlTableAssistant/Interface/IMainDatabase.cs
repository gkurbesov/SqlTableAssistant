using SqlTableAssistant.Arguments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTableAssistant.Interface
{
    /// <summary>
    /// Интерфейс для работы с базой данных
    /// </summary>
    public interface IMainDatabase
    {
        /// <summary>
        /// Соыбтие изменения состояние соейдинений
        /// </summary>
        event EventHandler<ConnectionArgs> OnConnectionStateChange;
        /// <summary>
        /// Событие об ошибках
        /// </summary>
        event EventHandler<ErrorArgs> OnDatabaseError;
        void CallConnectionStatus(StatusDataBase status);
        void CallError(string msg, string trace = "");
        /// <summary>
        /// Установка новых параметров
        /// </summary>
        /// <param name="value"></param>
        void SetConfig(ISqlConfig value);
        /// <summary>
        /// Подключиться к базе данных
        /// </summary>
        void Connect();
        /// <summary>
        /// Отключиться от базы данных
        /// </summary>
        void Disconnect();
        /// <summary>
        /// Проверка соединения с базой данных
        /// </summary>
        /// <returns></returns>
        bool IsConnected();
        /// <summary>
        /// Получить новый экземпляр подключения
        /// </summary>
        /// <param name="open">открыть соединение?</param>
        /// <returns></returns>
        IDbConnection GetNewConnection(bool open);
        /// <summary>
        /// Инициализировать Таблицы базы данных
        /// </summary>
        /// <param name="value">массив схем таблиц</param>
        void InitTables(IEnumerable<SchemasTable> value);
        /// <summary>
        /// Выполнить обычный запрос без ответа
        /// </summary>
        /// <param name="query">строка запроса</param>
        /// <returns></returns>
        bool ExecuteNonQuery(string query);
        /// <summary>
        /// Выполнить запросе без ответа
        /// </summary>
        /// <param name="query">строка запроса</param>
        /// <param name="query_params">параметры из строки запроса</param>
        /// <returns></returns>
        bool ExecuteNonQuery(string query, Dictionary<string, object> query_params);
        /// <summary>
        /// Выполнить вставку в таблицу
        /// </summary>
        /// <param name="table_name">имя таблицы</param>
        /// <param name="query_params">парметры втавки</param>
        /// <returns></returns>
        long ExecuteInsert(string table_name, Dictionary<string, object> query_params);
        /// <summary>
        /// Выполнить скалярный запрос
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        object ExecuteScalar(string query);
        /// <summary>
        /// ВЫполнить скалярный запрос с параметрами
        /// </summary>
        /// <param name="query">строк азапроса </param>
        /// <param name="query_params">параметры строки запроса</param>
        /// <returns></returns>
        object ExecuteScalar(string query, Dictionary<string, object> query_params);
        /// <summary>
        /// Выполнить выборку из базы
        /// </summary>
        /// <param name="query">строка запроса</param>
        /// <returns>DataSet содержащий результаты выборки</returns>
        DataSet ExecuteSelect(string query);
        /// <summary>
        /// Выполнить выборку из базы
        /// </summary>
        /// <param name="query">строка запроса</param>
        /// <param name="query_params">словарь параметров строки запроса</param>
        /// <returns></returns>
        DataSet ExecuteSelect(string query, Dictionary<string, object> query_params);
    }
}
