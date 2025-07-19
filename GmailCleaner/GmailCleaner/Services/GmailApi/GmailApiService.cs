using GmailCleaner.Services.GmailApi.Responses;
using RestSharp;
using System.Text.Json;

namespace GmailCleaner.Services.GmailApi
{
    public class GmailApiService
    {
        private readonly RestClient _gmailClient;
        
        public GmailApiService(
            GoogleAuthService authService)
        {
            _gmailClient = new RestClient(new RestClientOptions("https://gmail.googleapis.com/gmail/v1")
            {
                Authenticator = new GmailApiAuthenticator(authService)
            });
        }

        public async Task<MessageListResponse> ListMessagesAsync(string userId)
        {
            Console.WriteLine("Listing messages");
            var request = new RestRequest("users/{userId}/messages")
                .AddUrlSegment("userId", userId);

            var responseJson = (await _gmailClient.ExecuteGetAsync(request)).Content;

            var response = JsonSerializer.Deserialize<MessageListResponse>(responseJson);

            return response;
        }

        public async Task<List<MessageReponse>> GetMessagesAsync(string userId, IEnumerable<string> messageIds)
        {
            var messageBatches = messageIds.Chunk(20).ToArray();

            var responses = new List<MessageReponse>();
            
            foreach (var batch in messageBatches)
            {
                var tasks = batch.Select(id => GetMessageAsync(userId, id)).ToArray();
                
                var results = await Task.WhenAll(tasks);

                responses.AddRange(results);
            }
            
            return responses;
        }

        public async Task<MessageReponse> GetMessageAsync(string userId, string messageId)
        {
            var request = new RestRequest("users/{userId}/messages/{messageId}")
                .AddUrlSegment("userId", userId)
                .AddUrlSegment("messageId", messageId);
            
            var responseJson = (await _gmailClient.ExecuteGetAsync(request)).Content;
            
            return JsonSerializer.Deserialize<MessageReponse>(responseJson);
        }
    }
}
