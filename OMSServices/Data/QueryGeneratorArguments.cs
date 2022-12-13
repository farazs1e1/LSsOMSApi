using OMSServices.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSServices.Data
{
    public class QueryGeneratorArguments
    {
        public QueryType QueryType { get; set; }
        public List<string> Accounts { get; set; }
        public string UserDesc { get; set; }
        public string BoothID { get; set; }
        public string Symbol { get; set; }
        public RequestType RequestType { get; set; }
        public bool Fallback { get; set; }

        /// <summary>
        /// Optional Property
        /// </summary>
        public string UpdateQuery { get; set; }

        /// <summary>
        /// Optional Property
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Optional Property
        /// </summary>
        public string CustomQueryKey { get; set; }
    }
}
