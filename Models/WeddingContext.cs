using Microsoft.EntityFrameworkCore;

namespace csharpWeddings.Models
{
    public class WeddingContext : DbContext
    {
        // INCLUDE ALL MODELS AS DBSETS: ie. public DbSet<User> Users { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Wedding> Weddings { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public WeddingContext(DbContextOptions<WeddingContext> options) : base(options)
        { }
    }
}
