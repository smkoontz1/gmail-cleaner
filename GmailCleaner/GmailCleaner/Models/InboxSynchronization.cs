namespace GmailCleaner.Models;

public class InboxSynchronization
{
    public int InboxSynchronizationId { get; set; }
    public DateTime SyncDate { get; set; }
    public string HistoryId { get; set; }
}