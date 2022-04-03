namespace Ibanity.AccessToken;

public interface IAccessTokenManager
{
    IbanityToken? GetToken();
    public void FetchInitialToken(string authorizationCode);
    public void FetchRefreshToken(string refreshToken);
}

