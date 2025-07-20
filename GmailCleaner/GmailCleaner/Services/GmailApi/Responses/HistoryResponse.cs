using System.Text.Json.Serialization;

namespace GmailCleaner.Services.GmailApi.Responses;

public class HistoryResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("messagesAdded")]
    public List<MessageAddedResponse> MessagesAdded { get; set; }
}