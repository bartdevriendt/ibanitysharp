using System.ComponentModel;
using Newtonsoft.Json;

namespace Ibanity.Api;

[JsonConverter(typeof (JsonDotPathConverter))]
public class DocumentClient
{
    [JsonProperty("type")]
    [DefaultValue("client")]
    public string? Type { get; set; }

    [JsonProperty("id")]
    public string? Id { get; set; }
}