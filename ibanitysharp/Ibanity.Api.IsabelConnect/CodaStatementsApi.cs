using System.Security.Cryptography.X509Certificates;
using Ibanity.Certificates;
using Ibanity.AccessToken;
using Ibanity.Utils;
using Newtonsoft.Json;
using RestSharp;


namespace Ibanity.Api.IsabelConnect;

public class CodaStatementsApi
{
    private IAccessTokenManager _accessTokenManager;
    private ICertificateStore _certificateStore;
    private string _signingCertificateIdentifier;
    
    
    private const string ACCOUNT_REPORTS_URI = "/isabel-connect/account-reports";
    
    public CodaStatementsApi(IAccessTokenManager accessTokenManager, ICertificateStore certificateStore, string signingCertificateIdentifier)
    {
        _accessTokenManager = accessTokenManager;
        _certificateStore = certificateStore;
        _signingCertificateIdentifier = signingCertificateIdentifier;
    }
    
    public AccountReportList GetAccountReportList(string after = "")
    {
        
        string uri = ACCOUNT_REPORTS_URI;
        if (after != "0")
        {
            uri += $"?after={after}";
        }
        
        RestClientOptions options = new RestClientOptions( Constants.API_URL_PREFIX + uri);
        options.ClientCertificates = new X509CertificateCollection()
        {
            _certificateStore.GetAccessCertificate()
        };
        RestClient restClient = new RestClient(options);

        
        
        RestRequest restRequest = HttpRequestBuilder.NewBuilder()
            .WithMethod(Method.Get)
            .WithAccessToken(_accessTokenManager.GetToken().AccessToken)
            .WithRequestUri(uri)
            .WithSigningCertificate(_certificateStore.GetSigningCertificate())
            .WithSigningCertificateIdentifier(_signingCertificateIdentifier)
            .Build();

        var result = Task.Run<RestResponse>(async () => await restClient.ExecuteAsync(restRequest));
        RestResponse restResponse = result.Result;
        return restResponse.IsSuccessful ? JsonConvert.DeserializeObject<AccountReportList>(restResponse.Content) : throw new IbanityTokenException(restResponse.Content);
    }

    public string GetAccountReport(string uri)
    {
        RestClientOptions options = new RestClientOptions(uri);
        options.ClientCertificates = new X509CertificateCollection()
        {
            _certificateStore.GetAccessCertificate()
        };
        RestClient restClient = new RestClient(options);

        RestRequest restRequest = HttpRequestBuilder.NewBuilder()
            .WithMethod(Method.Get)
            .WithAccessToken(_accessTokenManager.GetToken().AccessToken)
            .WithRequestUri(uri.Replace(Constants.API_URL_PREFIX, ""))
            .WithSigningCertificate(_certificateStore.GetSigningCertificate())
            .WithSigningCertificateIdentifier(_signingCertificateIdentifier)
            .WithAcceptType("text/plain")
            .Build();
        
        var result = Task.Run<RestResponse>(async () => await restClient.ExecuteAsync(restRequest));
        RestResponse restResponse = result.Result;
        return restResponse.IsSuccessful ? restResponse.Content : throw new IbanityTokenException(restResponse.Content);
    }
}