using SqlTableAssistant.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTableAssistant.Arguments
{
    public class ConnectionArgs : EventArgs
    {
        /// <summary>
        /// Настройки подключения к базе данных, с которыми связано событие
        /// </summary>
        public ISqlConfig Config { get; private set; } = null;
        /// <summary>
        /// Статус подключения к базе данных
        /// </summary>
        public StatusDataBase Status { get; private set; } = StatusDataBase.Close;
        public ConnectionArgs() { }
        public ConnectionArgs(ISqlConfig config, StatusDataBase status)
        {
            Config = config;
            Status = status;
        }
    }
}
