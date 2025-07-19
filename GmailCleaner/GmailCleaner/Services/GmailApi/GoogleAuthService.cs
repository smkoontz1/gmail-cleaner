using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Flurl;
using Microsoft.AspNetCore.Components;

namespace GmailCleaner.Services.GmailApi;

public class GoogleAuthService(
    ProtectedLocalStorage localStorage,
    NavigationManager navigation)
{
    public async Task<string> GetAuthTokenAsync()
    {
        var tokenResult = await localStorage.GetAsync<string>(Constants.LocalStorageKeys.GoogleAccessToken);
        var tokenExpiryResult = await localStorage.GetAsync<long>(Constants.LocalStorageKeys.GoogleAccessTokenExpiry);

        if (!tokenResult.Success || !tokenExpiryResult.Success)
        {
            await RefreshTokenAsync();
        }
        
        var tokenExpiry = tokenExpiryResult.Value;
        var now = DateTime.UtcNow.Millisecond;

        if (now > tokenExpiry)
        {
            await RefreshTokenAsync();
        }

        return tokenResult.Value;
    }

    private async Task RefreshTokenAsync()
    {
        var currentLocation = navigation.Uri;
        Console.WriteLine($"CURRENT LOCATION: {currentLocation}");
        await localStorage.SetAsync(Constants.LocalStorageKeys.PostRefreshPath, currentLocation);
        
        var authEndpoint = "https://accounts.google.com/o/oauth2/v2/auth"
            .SetQueryParams(new
            {
                client_id = "771093418835-kao6u6nmsqtmtbo0vuje4iar7k84a01k.apps.googleusercontent.com",
                redirect_uri = "https://localhost:7142/google/token",
                response_type = "token",
                scope = "https://mail.google.com/",
                include_granted_scopes = "true",
            });

        navigation.NavigateTo(authEndpoint);
    }
}