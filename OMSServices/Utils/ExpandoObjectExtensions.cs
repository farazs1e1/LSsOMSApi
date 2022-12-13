using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSServices.Utils
{
    public static class ExpandoObjectExtensions
    {
        public static TResult Get_Value<TResult>(this ExpandoObject expandoObject, string key)
        {
            var dictionary = expandoObject as IDictionary<string, object>;
            return (TResult)dictionary[key];
        }
    }

    public static class ObjectExtensions
    {
        public static TResult GetValue<TResult>(this object obj, string key)
        {
            var dictionary = obj as IDictionary<string, object>;
            return (TResult)dictionary[key];
        }
    }
}
