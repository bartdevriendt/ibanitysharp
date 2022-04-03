using System.Net;
using System.Security.Cryptography.X509Certificates;
using Ibanity.Api;
using Ibanity.Extensions;
using RestSharp;

namespace Ibanity.Utils;

public class HttpRequestBuilder
{

    private Method _method;
    private string _accessToken;
    private string _acceptType;
    private string _requestUri;
    private X509Certificate2 _signingCertificate;
    private string _signingCertificateIdentifier;
    public HttpRequestBuilder()
    {
        _method = Method.Get;
        _acceptType = "application/vnd.api+json";
    }

    public static HttpRequestBuilder NewBuilder()
    {
        return new HttpRequestBuilder();
    }
    
    

    public HttpRequestBuilder WithMethod(Method method)
    {
        _method = method;
        return this;
    }

    public HttpRequestBuilder WithAccessToken(string accessToken)
    {
        _accessToken = accessToken;
        return this;
    }

    public HttpRequestBuilder WithRequestUri(string requestUri)
    {
        _requestUri = requestUri;
        return this;
    }
    
    public HttpRequestBuilder WithSigningCertificate(X509Certificate2 signingCertificate)
    {
        _signingCertificate = signingCertificate;
        return this;
    }

    public HttpRequestBuilder WithSigningCertificateIdentifier(string signingCertificateIdentifier)
    {
        _signingCertificateIdentifier = signingCertificateIdentifier;
        return this;
    }

    public HttpRequestBuilder WithNoAcceptType()
    {
        _acceptType = "";
        return this;
    }
    
    public HttpRequestBuilder WithAcceptType(string acceptType)
    {
        _acceptType = acceptType;
        return this;
    }

    public RestRequest Build()
    {
        RestRequest restRequest = new RestRequest();
        restRequest.AddHeader("Authorization", "Bearer " + _accessToken);
        restRequest.Method = _method;

        if (_acceptType != "")
        {
            restRequest.AddHeader("Accept", _acceptType);    
        }
        
        

        var digest = "SHA-512=" + "".ToBase64Sha512();
        var request_target = $"{FromMethod()} {_requestUri}";
        var epoch = DateTimeOffset.Now.ToUnixTimeSeconds();
        var signing_string =
            $"(request-target): {request_target}\nhost: {Constants.API_HOST}\ndigest: {digest}\n(created): {epoch}";
        var signature =
            Utils.Signing.GenerateSignatureForData(signing_string, _signingCertificate);
        
        restRequest.AddHeader("Digest", digest);

        var headers = "(request-target) host digest (created)";
        var signature_header =
            $"keyId=\"{_signingCertificateIdentifier}\",created={epoch},algorithm=\"hs2019\",headers=\"{headers}\",signature=\"{signature}\"";

        restRequest.AddHeader("Signature", signature_header);

        return restRequest;
    }

    private string FromMethod()
    {
        switch (_method)
        {
            case Method.Get:
                return "get";
            default:
                return "get";
        }
    }
    
}