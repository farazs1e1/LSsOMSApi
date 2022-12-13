using System;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace OMSApi.Configurations
{
    public class MyCustomTypeProvider : DefaultDynamicLinqCustomTypeProvider
    {
        public override HashSet<Type> GetCustomTypes() => new[] { typeof(ExpandoObject) }.ToHashSet();
    }
}
