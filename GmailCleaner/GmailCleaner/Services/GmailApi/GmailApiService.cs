using RestSharp;
using RestSharp.Authenticators.OAuth2;
using System.Text.Json;

namespace GmailCleaner.Services.GmailApi
{
    public class GmailApiService
    {
        private readonly RestClient _gmailClient;

        public GmailApiService(string authToken)
        {
            _gmailClient = new RestClient(new RestClientOptions("https://gmail.googleapis.com/gmail/v1")
            {
                Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(authToken, "Bearer")
            });
        }

        public async Task<string> ListMessagesAsync(string userId)
        {
            var request = new RestRequest("users/{userId}/messages")
                .AddUrlSegment("userId", userId);

            var response = await _gmailClient.ExecuteGetAsync(request);

            return response.Content;
        }
    }
}
