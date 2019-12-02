using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AutomaticInvestmentPlan_Network
{
    public class WorkDayService
    {
        public bool WhetherWorkDay()
        {
            string url = "http://api.goseek.cn/Tools/holiday?date=" + DateTime.Now.ToString("yyyyMMdd");

            using (HttpClient client = new HttpClient())
            {
               Task<string> t = client.GetStringAsync(url);
                string r = t.Result;
                QueryResultModel m = QueryResultModel.FromJson(r);
                if (m.Data == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public partial class QueryResultModel
    {
        [JsonProperty("code", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? Code { get; set; }

        [JsonProperty("data", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? Data { get; set; }
    }

    public partial class QueryResultModel
    {
        public static QueryResultModel FromJson(string json) => JsonConvert.DeserializeObject<QueryResultModel>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this QueryResultModel self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}