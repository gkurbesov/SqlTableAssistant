using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTableAssistant.SQLite
{
    public class JournalModeType
    {
        public const string DELETE = "DELETE";
        public const string TRUNCATE = "TRUNCATE";
        public const string PERSIST = "PERSIST";
        public const string MEMORY = "MEMORY";
        public const string WAL = "WAL";
        public const string OFF = "OFF";
    }
}
