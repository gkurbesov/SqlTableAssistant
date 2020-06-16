using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTableAssistant
{
    /// <summary>
    /// Статусы соединений
    /// </summary>
    public enum StatusDataBase
    {
        /// <summary>
        /// Соединение закрыто
        /// </summary>
        Close,
        /// <summary>
        /// Подключение
        /// </summary>
        Connecting,
        /// <summary>
        /// Подключено
        /// </summary>
        Connect,
        /// <summary>
        /// Ошибка закрытия соединения
        /// </summary>
        ErrorCloseConnecting,
        /// <summary>
        /// Ошибка подключения
        /// </summary>
        ErrorConnecting,
        /// <summary>
        /// Ошибка записи в БД
        /// </summary>
        ErrorWrite,
        /// <summary>
        /// Ошибка чтения из БД
        /// </summary>
        ErrorRead
    }
}
