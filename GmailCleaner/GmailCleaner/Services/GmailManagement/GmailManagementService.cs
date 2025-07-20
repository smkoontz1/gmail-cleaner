using System.Diagnostics;
using GmailCleaner.Data.Gmail;
using GmailCleaner.Models;
using GmailCleaner.Models.SenderCounts;
using GmailCleaner.Services.GmailApi;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.Services.GmailManagement;

public class GmailManagementService(GmailApiService gmailApiService)
{
    public async Task SyncMail()
    {
        using (var context = new GmailDbContext())
        {
            await context.Database.EnsureCreatedAsync();
            
            var mostRecentSync = await context.InboxSynchronizations
                .OrderByDescending(s => s.SyncDate)
                .FirstOrDefaultAsync();

            var userId = Constants.GmailUserId;
            string? nextHistoryId = null;
            
            if (mostRecentSync is not null)
            {
                Debug.WriteLine("Partial sync");
                var startHistoryId = mostRecentSync.HistoryId;
                string? nextPageToken = null;
                int pageCount = 0;
                
                do
                {
                    Debug.WriteLine($"Syncing page {pageCount}");
                    
                    var historyListPage = await gmailApiService.ListHistoryAsync(
                        userId,
                        startHistoryId,
                        nextPageToken);

                    var messageIdsAdded = new List<string>();
                    historyListPage.History.ForEach(h =>
                    {
                        messageIdsAdded.AddRange(h.MessagesAdded.Select(m => m.Message.Id));
                    });
                    var messages = await gmailApiService.GetMessagesAsync(userId, messageIdsAdded);

                    var storedMessages = messages.Select(m => new StoredMessage
                    {
                        GmailMessageId = m.Id,
                        HistoryId = m.HistoryId,
                        InternalDate = long.Parse(m.InternalDate),
                        Sender = m.Payload.Headers.First(h => h.Name == "From").Value,
                        Subject = m.Payload.Headers.First(h => h.Name == "Subject").Value
                    });

                    context.Messages.AddRange(storedMessages);
                    await context.SaveChangesAsync();

                    nextHistoryId = historyListPage.HistoryId;
                    nextPageToken = historyListPage.NextPageToken;
                } while (nextPageToken is not null);
            }
            else
            {
                Debug.WriteLine("Initial sync");
                string? nextPageToken = null;
                int pageCount = 0;

                do
                {
                    pageCount++;
                    Debug.WriteLine($"Syncing page {pageCount}");
                    
                    var messageListPage = await gmailApiService.ListMessagesAsync(
                        userId,
                        nextPageToken);

                    var messageIds = messageListPage.Messages.Select(m => m.Id).ToList();
                    Debug.WriteLine($"Retreived {messageIds.Count} message ids from list");
                    var messages = await gmailApiService.GetMessagesAsync(userId, messageIds);
                    Debug.WriteLine($"Retrieved {messages.Count} message details for ids");
                    
                    var storedMessages = messages.Select(m => new StoredMessage
                    {
                        GmailMessageId = m.Id,
                        HistoryId = m.HistoryId,
                        InternalDate = long.Parse(m.InternalDate),
                        Sender = m.Payload.Headers.FirstOrDefault(h => h.Name == "From")?.Value,
                        Subject = m.Payload.Headers.FirstOrDefault(h => h.Name == "Subject")?.Value
                    })
                    .ToList();

                    context.Messages.AddRange(storedMessages);
                    await context.SaveChangesAsync();

                    Debug.WriteLine($"Stored {storedMessages.Count} messages in the database");
                    
                    if (pageCount == 1)
                    {
                        nextHistoryId = messages.First().HistoryId;
                        Debug.WriteLine($"Next history id is: {nextHistoryId}");
                    }

                    nextPageToken = messageListPage.NextPageToken;
                } while (nextPageToken is not null);
            }
            
            context.InboxSynchronizations.Add(new()
            {
                HistoryId = nextHistoryId,
                SyncDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }
    }
    
    public async Task<SenderCountPage> GetSenderCountsAsync(int pageNumber, int pageSize)
    {
        using (var context = new GmailDbContext())
        {
            await context.Database.EnsureCreatedAsync();
            
            var queryable = context.Messages
                .Where(m => !m.IsDeleted)
                .GroupBy(m => m.Sender)
                .Select(g => new SenderCount
                {
                    Sender = g.Key,
                    Count = g.Count()
                });

            var page = await queryable
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.Sender)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var totalCount = await queryable.CountAsync();

            return new()
            {
                Page = page,
                TotalCount = totalCount,
            };
        }
    }

    public async Task DeleteBySendersAsync(IEnumerable<string> senders)
    {
        using (var context = new GmailDbContext())
        {
            var messages = await context.Messages
                .Where(m => senders.Contains(m.Sender))
                .ToListAsync();

            var messageIds = messages.Select(m => m.GmailMessageId);

            var success = await gmailApiService.BatchDeleteAsync(Constants.GmailUserId, messageIds);
            if (success)
            {
                messages.ForEach(m => m.IsDeleted = true);

                await context.SaveChangesAsync();
            }
        }
    }
}