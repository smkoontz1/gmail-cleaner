using GmailCleaner.Data.Gmail.Models;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.Data.Gmail
{
    public class GmailDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public string DbPath { get; }

        public GmailDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "gmail.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
