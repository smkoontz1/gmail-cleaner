namespace GmailCleaner.Data.Gmail.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public string Subject { get; set; } = default!;
        public string Sender { get; set; } = default!;
        public string Body { get; set; } = default!;
    }
}
