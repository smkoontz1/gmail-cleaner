using GmailCleaner.Models;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.Data.Gmail
{
    public class GmailDbContext : DbContext
    {
        public DbSet<StoredMessage> Messages { get; set; }
        public DbSet<InboxSynchronization> InboxSynchronizations { get; set; }

        public string DbPath { get; }

        public GmailDbContext()
        {
            var localAppDataFolder = Environment.SpecialFolder.LocalApplicationData;
            var localAppDataPath = Environment.GetFolderPath(localAppDataFolder);
            DbPath = Path.Join(
                localAppDataPath,
                Constants.AppName,
                "gmail.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
