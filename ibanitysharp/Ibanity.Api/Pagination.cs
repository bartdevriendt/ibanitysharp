using Newtonsoft.Json;

namespace Ibanity.Api;

[JsonConverter(typeof (JsonDotPathConverter))]
public class Pagination
{
    [JsonProperty("after")]
    public string? After { get; set; }

    [JsonProperty("limit")]
    public int? Limit { get; set; }
}
