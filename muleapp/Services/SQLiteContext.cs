using Microsoft.EntityFrameworkCore;

namespace Mule
{
    public class SQLiteContext : DbContext
    {
        public SQLiteContext(DbContextOptions<SQLiteContext> options) : base(options)
        { }

        public DbSet<AppHost> AppHosts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
