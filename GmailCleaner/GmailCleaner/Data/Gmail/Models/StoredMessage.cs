namespace GmailCleaner.Data.Gmail.Models
{
    public class StoredMessage
    {
        public int StoredMessageId { get; set; }
        public string GmailMessageId { get; set; }
        public string Sender { get; set; } = default!;
        public string Subject { get; set; } = default!;
    }
}
