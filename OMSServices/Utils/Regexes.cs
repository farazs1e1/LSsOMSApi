using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSServices.Utils
{
    public static class Regexes
    {
        public const string Symbol = @"^[A-Z0-9]{1,9}(\.[A-Z0-9]{1,9})?(-[A-Z0-9]{1,4})?$";
    }
}
