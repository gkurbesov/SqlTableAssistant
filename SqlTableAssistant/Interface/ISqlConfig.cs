using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTableAssistant.Interface
{
    public interface ISqlConfig
    {
        /// <summary>
        /// Получить строку для подключения к БД
        /// </summary>
        /// <returns></returns>
        string GetConnectionString();
    }
}
