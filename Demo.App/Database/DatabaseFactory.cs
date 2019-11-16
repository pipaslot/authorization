using Microsoft.EntityFrameworkCore;

namespace Demo.App.Database
{
    public class DatabaseFactory
    {
        private DbContextOptions<AppDbContext> _options;

        public DatabaseFactory(string connectionString)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseSqlServer(connectionString);
            _options = builder.Options;
        }

        public AppDbContext Create()
        {
            return new AppDbContext(_options);
        }
    }
}
