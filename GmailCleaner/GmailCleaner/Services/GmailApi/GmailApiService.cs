using GmailCleaner.Services.GmailApi.Responses;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using System.Diagnostics;
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

        public async Task<MessageListResponse> ListMessagesAsync(string userId)
        {
            var request = new RestRequest("users/{userId}/messages")
                .AddUrlSegment("userId", userId);

            var responseJson = (await _gmailClient.ExecuteGetAsync(request)).Content;

            Debug.WriteLine(responseJson);

            var response = JsonSerializer.Deserialize<MessageListResponse>(responseJson);

            return response;
        }
    }
}
