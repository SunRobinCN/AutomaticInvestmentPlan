using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AutomaticInvestmentPlan_Comm
{
    public partial class HttpResultModel
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("type")]
        public TypeClass Type { get; set; }

        [JsonProperty("holiday")]
        public Holiday Holiday { get; set; }
    }

    public partial class Holiday
    {
        [JsonProperty("holiday")]
        public bool HolidayHoliday { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("wage")]
        public long Wage { get; set; }

        [JsonProperty("after")]
        public bool After { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }
    }

    public partial class TypeClass
    {
        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("week")]
        public long Week { get; set; }
    }

    public partial class HttpResultModel
    {
        public static HttpResultModel FromJson(string json) =>
            JsonConvert.DeserializeObject<HttpResultModel>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this HttpResultModel self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            },
        };
    }
}