using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSServices.Querry
{
    public struct QueryData
    {
        public string Columns { get; set; }
        public string Provider { get; set; }
        public string Cache { get; set; }
        public string Table { get; set; }
        public string Condition { get; set; }
        public string Predicate { get; set; }
        public bool ApplyAccountsConstraints { get; internal set; }
    }
}
