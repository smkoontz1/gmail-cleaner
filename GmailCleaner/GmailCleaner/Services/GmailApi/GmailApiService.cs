using System.Diagnostics;
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

        public async Task<HistoryListResponse> ListHistoryAsync(
            string userId,
            string startHistoryId,
            string? pageToken = null)
        {
            var request = new RestRequest("users/{userId}/history")
                .AddUrlSegment("userId", userId)
                .AddQueryParameter("maxResults", 500)
                .AddQueryParameter("startHistoryId", startHistoryId)
                .AddQueryParameter("historyTypes", "messageAdded" );

            if (!string.IsNullOrWhiteSpace(pageToken))
            {
                request.AddQueryParameter("pageToken", pageToken);
            }

            var apiResponse = await _gmailClient.ExecuteGetAsync(request);
            if (!apiResponse.IsSuccessful)
            {
                Debug.WriteLine($"List history failed with code: {apiResponse.StatusCode}");
            }
            var responseJson = apiResponse.Content;

            var response = JsonSerializer.Deserialize<HistoryListResponse>(responseJson);

            return response;
        }
        
        public async Task<MessageListResponse> ListMessagesAsync(
            string userId,
            string? pageToken = null)
        {
            var request = new RestRequest("users/{userId}/messages")
                .AddUrlSegment("userId", userId)
                .AddQueryParameter("maxResults", 500);

            if (!string.IsNullOrWhiteSpace(pageToken))
            {
                request.AddQueryParameter("pageToken", pageToken);
            }

            var apiResponse = await _gmailClient.ExecuteGetAsync(request);
            if (!apiResponse.IsSuccessful)
            {
                Debug.WriteLine($"List messages failed with code: {apiResponse.StatusCode}");
            }
            var responseJson = apiResponse.Content;

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
                Debug.WriteLine($"Retrieved {responses.Count} messages for page");
            }
            
            return responses;
        }

        public async Task<MessageReponse> GetMessageAsync(string userId, string messageId)
        {
            var request = new RestRequest("users/{userId}/messages/{messageId}")
                .AddUrlSegment("userId", userId)
                .AddUrlSegment("messageId", messageId);

            var apiResponse = await _gmailClient.ExecuteGetAsync(request);
            if (!apiResponse.IsSuccessful)
            {
                Debug.WriteLine($"Get message failed with code: {apiResponse.StatusCode}");
            }
            var responseJson = apiResponse.Content;
            
            return JsonSerializer.Deserialize<MessageReponse>(responseJson);
        }

        public async Task<bool> BatchDeleteAsync(string userId, IEnumerable<string> messageIds)
        {
            var messageIdBatches = messageIds.Chunk(500).ToArray();

            var success = true;
            
            foreach (var batch in messageIdBatches)
            {
                var request = new RestRequest("users/{userId}/messages/batchDelete")
                    .AddUrlSegment("userId", userId)
                    .AddBody(new
                    {
                        ids = batch.ToArray()
                    });
                
                var response = await _gmailClient.ExecutePostAsync(request);
                if (!response.IsSuccessful)
                {
                    success = false;
                    Debug.WriteLine($"Batch delete failed with code: {response.StatusCode}");
                }
            }

            return success;
        }
    }
}
