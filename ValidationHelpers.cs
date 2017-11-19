using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dac.CodeAnalysis;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Text.RegularExpressions;

namespace CustomCodeAnalysisRule
{
    class ValidationHelpers
    {
        public static bool IsDWHTimeStamp(string value)
        {
            var regEx = new Regex(GetRegularExpression("DWHTimestamp"));

            return regEx.IsMatch(FixSpecialAcronyms(value));
        }
        private static string GetRegularExpression(string prefix)
        {
            return $@"\b^{prefix}[A-Z][a-z]*((_[A-Z]|[A-Z])[a-z]+)*\b";
        }

        private static string FixSpecialAcronyms(string value)
        {
            return value.Replace("PnL", "Pnl");
        }
    }
}
