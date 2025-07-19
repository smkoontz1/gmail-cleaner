using RestSharp;
using RestSharp.Authenticators;

namespace GmailCleaner.Services.GmailApi;

public class GmailApiAuthenticator(GoogleAuthService authService) : AuthenticatorBase("")
{
    protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken)
    {
        Token = string.IsNullOrEmpty(Token) ? await authService.GetAuthTokenAsync() : Token;

        return new HeaderParameter(KnownHeaders.Authorization, $"Bearer {Token}");
    }
}