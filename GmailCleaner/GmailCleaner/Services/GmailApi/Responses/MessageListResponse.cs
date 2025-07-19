using System.Text.Json.Serialization;

namespace GmailCleaner.Services.GmailApi.Responses
{
    public class MessageListResponse
    {
        [JsonPropertyName("messages")]
        public List<MessageReponse> Messages { get; set; }
    }
}
