using RestSharp;
using RestSharp.Authenticators;

namespace GmailCleaner.Services.GmailApi;

public class GmailApiAuthenticator(GoogleAuthService authService) : AuthenticatorBase("")
{
    protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken)
    {
        Token = await authService.GetAuthTokenAsync();

        return new HeaderParameter(KnownHeaders.Authorization, $"Bearer {Token}");
    }
}