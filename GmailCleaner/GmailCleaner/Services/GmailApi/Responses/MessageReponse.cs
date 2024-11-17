using System.Text.Json.Serialization;

namespace GmailCleaner.Services.GmailApi.Responses
{
    public class MessageReponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("threadId")]
        public string ThreadId { get; set; }
    }
}
