using OMSServices.Enum;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSServices.Utils
{
    internal static class Bin
    {
        public static void Dump(string queryType, ExpandoObject data)
        {
            if (QueryType.Locates.ToString().Equals(queryType, StringComparison.Ordinal))
            {
                DumpToFile("./logs/locates-logs.txt", data);
            }
            else if (QueryType.LocateSummary.ToString().Equals(queryType, StringComparison.Ordinal))
            {
                DumpToFile("./logs/locate_summary-logs.txt", data);
            }
            else if (QueryType.AuditTrail.ToString().Equals(queryType, StringComparison.Ordinal))
            {
                DumpToFile("./logs/audit_trail-logs.txt", data);
            }
            else if (QueryType.Positions.ToString().Equals(queryType, StringComparison.Ordinal))
            {
                DumpToFile("./logs/positions-logs.txt", data);
            }
            else if (QueryType.Executions.ToString().Equals(queryType, StringComparison.Ordinal))
            {
                DumpToFile("./logs/executions-logs.txt", data);
            }
            else if (QueryType.Orders.ToString().Equals(queryType, StringComparison.Ordinal))
            {
                DumpToFile("./logs/orders-logs.txt", data);
            }
            else if (QueryType.OpenOrders.ToString().Equals(queryType, StringComparison.Ordinal))
            {
                DumpToFile("./logs/openorders-logs.txt", data);
            }
            else if (QueryType.OptionOrders.ToString().Equals(queryType, StringComparison.Ordinal))
            {
                DumpToFile("./logs/optionorders-logs.txt", data);
            }
        }

        #region PRIVATE METHODS

        private static void DumpToFile(string filePath, ExpandoObject data)
        {
            string serializedData = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
            });
            string text = $"\n\n{DateTime.Now}\n{serializedData}\n\n";
            //lock (GetLock(filePath))
            {
                File.AppendAllText(filePath, text);
            }
        }

        #endregion
    }
}
