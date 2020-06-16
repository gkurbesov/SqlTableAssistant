﻿using SqlTableAssistant.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTableAssistant.MySql
{
    public class MySqlConfig : ISqlConfig
    {
        public string Server { get; set; } = "127.0.0.1";
        public string Port { get; set; } = "3306";
        public string Database { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Charset { get; set; } = "utf8";
        public bool Pooling { get; set; } = true;

        public MySqlConfig() { }

        public MySqlConfig(string server, string port)
        {
            Server = server;
            Port = port;
        }

        public MySqlConfig SetServer(string value)
        {
            Server = value;
            return this;
        }

        public MySqlConfig SetPort(string value)
        {
            Port = value;
            return this;
        }

        public MySqlConfig SetPort(int value)
        {
            Port = value.ToString();
            return this;
        }

        public MySqlConfig SetDatabase(string value)
        {
            Database = value;
            return this;
        }
        public MySqlConfig SetUser(string value)
        {
            User = value;
            return this;
        }
        public MySqlConfig SetPassword(string value)
        {
            Password = value;
            return this;
        }

        public MySqlConfig SetCharset(string value)
        {
            Charset = value;
            return this;
        }

        public MySqlConfig SetPooling(bool value)
        {
            Pooling = value;
            return this;
        }

        public string GetConnectionString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Server=");
            sb.Append(Server);
            sb.Append("; Port=");
            sb.Append(Port);
            sb.Append("; Database=");
            sb.Append(Database);
            sb.Append("; Uid=");
            sb.Append(User);
            sb.Append("; Pwd=");
            sb.Append(Password);
            sb.Append("; charset=");
            sb.Append(Charset);
            sb.Append("; Pooling=");
            sb.Append(Pooling.ToString());
            sb.Append(";");
            return sb.ToString();
        }
    }
}
