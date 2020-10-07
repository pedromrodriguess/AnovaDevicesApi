using DevicesApi.Data;
using Microsoft.EntityFrameworkCore;

namespace DevicesApi.UnitTests
{
    public static class DbContextMocker
    {
        public static ApplicationDbContext GetDbContext(string dbName)
        {
            // Create options for DbContext instance
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            // Create instance of DbContext
            var dbContext = new ApplicationDbContext(options);

            // Add entities in memory
            dbContext.Seed();

            return dbContext;
        }
    }
}
