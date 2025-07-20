using System.Text.Json.Serialization;

namespace GmailCleaner.Services.GmailApi.Responses;

public class MessageAddedResponse
{
    [JsonPropertyName("message")]
    public MessageReponse Message { get; set; }
}