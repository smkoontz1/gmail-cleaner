namespace GmailCleaner.Models.SenderCounts;

public class SenderCountPage
{
    public List<SenderCount> Page { get; set; } = new();
    public int TotalCount { get; set; }
}