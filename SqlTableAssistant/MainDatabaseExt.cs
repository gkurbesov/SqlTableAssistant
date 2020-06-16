using SqlTableAssistant.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTableAssistant
{
    public static class MainDatabaseExt
    {
        public static string InsertString(this IMainDatabase db, string table_name, Dictionary<string, object> query_params)
        {
            string param1 = string.Join(",", query_params.Select((key, val) => key).ToArray());
            string param2 = string.Join(",", query_params.Select((key, val) => $"@{key}").ToArray());
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(table_name);
            sb.Append(" (");
            sb.Append(param1);
            sb.Append(") VALUES (");
            sb.Append(param2);
            sb.Append(")");
            return sb.ToString();
        }
    }
}
