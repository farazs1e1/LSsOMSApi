using System.Collections.Generic;

namespace OMSServices.Models
{
    public class TraderRelationWith
    {
        public string BoothID { get; set; }
        public string ID { get; set; }
        public string Key { get; set; }
        public string RecordID { get; set; }
        public long SeekInfo { get; set; }
        public string Username { get; set; }
    }

    public static class TraderRelationWithExtensions
    {
        public static IEnumerable<TraderRelationWith> GetTraderRelationWithValues(this object enumerable)
        {
            foreach (IDictionary<string, object> obj in enumerable as IEnumerable<object>)
            {
                yield return new TraderRelationWith
                {
                    BoothID = obj["BoothID"] as string,
                    ID = obj["ID"] as string,
                    Key = obj["Key"] as string,
                    RecordID = obj["RecordID"] as string,
                    SeekInfo = (long)obj["SeekInfo"],
                    Username = obj["Username"] as string,
                };
            }
        }
    }
}
