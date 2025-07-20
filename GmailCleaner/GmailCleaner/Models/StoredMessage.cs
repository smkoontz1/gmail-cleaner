namespace GmailCleaner.Models
{
    public class StoredMessage
    {
        public int StoredMessageId { get; set; }
        public string GmailMessageId { get; set; }
        public string HistoryId { get; set; }
        public long InternalDate { get; set; }
        public string? Sender { get; set; } = default!;
        public string? Subject { get; set; } = default!;
        public bool IsDeleted { get; set; }
    }
}
