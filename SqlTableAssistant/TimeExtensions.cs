using System;
using System.Collections.Generic;
using System.Text;

namespace SqlTableAssistant
{
    public static class TimeExtensions
    {
        public static string ToStringDatabase(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
