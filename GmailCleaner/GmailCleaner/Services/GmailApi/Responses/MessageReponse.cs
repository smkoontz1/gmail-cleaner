using System.Text.Json.Serialization;

namespace GmailCleaner.Services.GmailApi.Responses
{
    public class MessageReponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("threadId")]
        public string ThreadId { get; set; }
        
        [JsonPropertyName("snippet")]
        public string Snippet { get; set; }
        
        [JsonPropertyName("historyId")]
        public string HistoryId { get; set; }
        
        [JsonPropertyName("internalDate")]
        public string InternalDate { get; set; }
        
        [JsonPropertyName("payload")]
        public PayloadResponse Payload { get; set; }
    }

    public class PayloadResponse
    {
        [JsonPropertyName("headers")]
        public HeaderReponse[] Headers { get; set; }
    }
    
    public class HeaderReponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
