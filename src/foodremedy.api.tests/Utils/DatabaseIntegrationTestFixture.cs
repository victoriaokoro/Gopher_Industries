using foodremedy.database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace foodremedy.api.tests.Utils;

public abstract class DatabaseIntegrationTestFixture
{
    protected async Task RunInScopeAsync<TSut>(Func<FoodRemedyDbContext, TSut> systemUnderTestFactory, Func<TSut, Task> inScope)
    {
        await RunInScopeAsync(systemUnderTestFactory, (sut, _) => inScope(sut));
    }
    
    protected async Task RunInScopeAsync<TSut>(Func<FoodRemedyDbContext, TSut> systemUnderTestFactory, Func<TSut, FoodRemedyDbContext, Task> inScope)
    {
        await using var context = new TestContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await inScope.Invoke(systemUnderTestFactory.Invoke(context), context);
    }

    internal class TestContext : FoodRemedyDbContext
    {
        private readonly SqliteConnection _connection = new("Filename=:memory:");

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            _connection.Open();
            builder.UseSqlite(_connection);
        }

        public override void Dispose()
        {
            _connection.Dispose();
            base.Dispose();
        }
    }
}
