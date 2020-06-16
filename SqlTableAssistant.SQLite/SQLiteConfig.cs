using SqlTableAssistant.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTableAssistant.SQLite
{
    public class SQLiteConfig : ISqlConfig
    {
        public string Path { get; set; } = string.Empty;
        public string File { get; set; } = string.Empty;
        public string Synchronous { get; set; } = SynchronousType.OFF;
        public string JournalMode { get; set; } = JournalModeType.OFF;
        public string AutoVacuum { get; set; } = AutoVacuumType.NONE;

        public SQLiteConfig() { }

        public SQLiteConfig(string path, string file_name)
        {
            Path = path;
            File = file_name;
        }

        public SQLiteConfig SetPath(string value)
        {
            Path = value;
            return this;
        }

        public SQLiteConfig SetFileName(string value)
        {
            File = value;
            return this;
        }

        public SQLiteConfig SetSynchronous(string value)
        {
            Synchronous = value;
            return this;
        }

        public SQLiteConfig SetJournalMode(string value)
        {
            JournalMode = value;
            return this;
        }

        public SQLiteConfig SetAutoVacuum(string value)
        {
            AutoVacuum = value;
            return this;
        }

        private string GetPathFile()
        {
            return Path.TrimEnd('\\') + $"\\{File}.db";
        }
        /// <summary>
        /// Получить строку подключения к базе данных
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Data Source='");
            sb.Append(GetPathFile());
            sb.Append("'; Version=3; synchronous=");
            sb.Append(Synchronous);
            sb.Append("; journal_mode=");
            sb.Append(JournalMode);
            sb.Append("; auto_vacuum=");
            sb.Append(AutoVacuum);
            sb.Append(";");
            return sb.ToString();

        }
    }
}
