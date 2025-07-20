using System.Text.Json.Serialization;

namespace GmailCleaner.Services.GmailApi.Responses;

public class HistoryListResponse
{
    [JsonPropertyName("history")]
    public List<HistoryResponse> History { get; set; }
    
    [JsonPropertyName("nextPageToken")]
    public string? NextPageToken { get; set; }
    
    [JsonPropertyName("historyId")]
    public string HistoryId { get; set; }
}