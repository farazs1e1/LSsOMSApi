using System.Collections.Generic;

namespace OMSServices.Models
{
    public class StaticDataValues
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Booth { get; set; }
        internal bool IsDefault { get; set; }
    }

    public static class StaticDataValuesExtension
    {
        public static IEnumerable<StaticDataValues> GetStaticDataValues(this object enumerable)
        {
            var objects = enumerable as IEnumerable<object>;

            foreach (IDictionary<string, object> obj in objects)
            {
                yield return new StaticDataValues
                {
                    Name = obj["Name"] as string,
                    Value = obj["Value"] as string,
                    Booth = obj.ContainsKey("Booth") ? obj["Booth"] as string : null,
                };
            }
        }
    }
}
