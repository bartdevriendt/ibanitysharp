using Newtonsoft.Json;

namespace Ibanity.AccessToken;

public class IbanityTokenException : Exception
{
    public IbanityTokenException(string content)
    {
        IbanityTokenException.ErrorResponse errorResponse = JsonConvert.DeserializeObject<IbanityTokenException.ErrorResponse>(content);
        this.Error = errorResponse?.Error ?? "Failed to read error";
        this.Description = errorResponse?.ErrorDescription ?? "Failed to read error";
    }

    public string Error { get; private set; }

    public string Description { get; private set; }

    private class ErrorResponse
    {
        [JsonProperty("error")]
        public string? Error { get; set; }

        [JsonProperty("error_description")]
        public string? ErrorDescription { get; set; }
    }
}