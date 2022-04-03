using System.Security.Cryptography.X509Certificates;
using Ibanity.Certificates;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace Ibanity.AccessToken;

public class IsabelConnectAccessTokenManager : IAccessTokenManager
  {
    private readonly string _clientId;
    private readonly string _clientSecret;
    
    private readonly ICertificateStore _certificateSource;
    private const string ACCESSTOKENURL = "https://api.ibanity.com/isabel-connect/oauth2/token";
    private static IbanityToken? _token;

    public IsabelConnectAccessTokenManager(ICertificateStore certificateSource, string clientId, string clientSecret)
    {
      this._clientId = clientId;
      this._clientSecret = clientSecret;
      this._certificateSource = certificateSource;
    }

    public IbanityToken? GetToken()
    {
        return _token;
    }


    public void FetchInitialToken(string authorizationCode)
    {
        RestClientOptions options = new RestClientOptions(ACCESSTOKENURL);
        options.ClientCertificates = new X509CertificateCollection()
        {
          (X509Certificate) this._certificateSource.GetAccessCertificate()
        };
        RestClient restClient = new RestClient(options);
        restClient.Authenticator = (IAuthenticator) new HttpBasicAuthenticator(this._clientId, this._clientSecret);
        RestRequest restRequest = new RestRequest();
        restRequest.Method = Method.Post;

        string body = $"grant_type=authorization_code&code={authorizationCode}&redirect_uri=http://localhost";
        restRequest.AddStringBody(body, DataFormat.None);
        
        restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        restRequest.AddHeader("Accept", "application/vnd.api+json");

        var result = Task.Run<RestResponse>(async () => await restClient.ExecuteAsync(restRequest));
        RestResponse restResponse = result.Result;
        _token = restResponse.IsSuccessful ? JsonConvert.DeserializeObject<IbanityToken>(restResponse.Content) : throw new IbanityTokenException(restResponse.Content);
    }

    public void FetchRefreshToken(string refreshToken)
    {
        RestClientOptions options = new RestClientOptions(ACCESSTOKENURL);
        options.ClientCertificates = new X509CertificateCollection()
        {
          (X509Certificate) this._certificateSource.GetAccessCertificate()
        };
        RestClient restClient = new RestClient(options);
        restClient.Authenticator = (IAuthenticator) new HttpBasicAuthenticator(this._clientId, this._clientSecret);
        RestRequest restRequest = new RestRequest();
        restRequest.Method = Method.Post;

        string body = $"grant_type=refresh_token&refresh_token={refreshToken}";
        restRequest.AddStringBody(body, DataFormat.None);
          
        restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        restRequest.AddHeader("Accept", "application/vnd.api+json");

        var result = Task.Run<RestResponse>(async () => await restClient.ExecuteAsync(restRequest));
        RestResponse restResponse = result.Result;
        _token = restResponse.IsSuccessful ? JsonConvert.DeserializeObject<IbanityToken>(restResponse.Content) : throw new IbanityTokenException(restResponse.Content);
    }

  }