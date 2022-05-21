namespace Ibanity.AccessToken;

public interface IAccessTokenManager
{
    IbanityToken? GetToken();
    public void FetchInitialToken(string authorizationCode, string redirectUrl);
    public void FetchRefreshToken(string refreshToken);
}

