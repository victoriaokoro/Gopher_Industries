using System.Net.Http.Headers;
using System.Net.Http.Json;
using foodremedy.api.Models.Requests;
using foodremedy.api.Models.Responses;
using foodremedy.database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace foodremedy.api.tests.Controllers;

public abstract class ControllerTestFixture
{
    protected HttpClient _webApiClient;
    private SqliteConnection _sqliteConnection;
    private IServiceScope _scope;
    protected FoodRemedyDbContext DbContext;
    
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _sqliteConnection = new SqliteConnection("Filename=:memory:");
        _sqliteConnection.Open();
        
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    ServiceDescriptor? descriptor = services.SingleOrDefault(p =>
                        p.ServiceType == typeof(DbContextOptions<FoodRemedyDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<FoodRemedyDbContext>(config =>
                    {
                        config.UseSqlite(_sqliteConnection);
                    });
                });
            });
        _webApiClient = factory.CreateClient();

        _scope = factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<FoodRemedyDbContext>();
        DbContext.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        DbContext.Database.ExecuteSqlRaw("DELETE FROM User");
        DbContext.Database.ExecuteSqlRaw("DELETE FROM RefreshToken");
        DbContext.Database.ExecuteSqlRaw("DELETE FROM Ingredient");
        DbContext.Database.ExecuteSqlRaw("DELETE FROM Tag");
        DbContext.Database.ExecuteSqlRaw("DELETE FROM TagCategory");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _sqliteConnection.Dispose();
        _webApiClient.Dispose();
        DbContext.Dispose();
        _scope.Dispose();
    }

    protected async Task<HttpResponseMessage> SendAuthenticatedRequestAsync(HttpRequestMessage requestMessage)
    {
        const string email = "test@test.com";
        const string password = "password";
        
        await _webApiClient.PostAsync("/users/register", JsonContent.Create(new RegisterUser(email, password)));
        var response =
            await _webApiClient.PostAsync("auth/login", JsonContent.Create(new AttemptLogin(email, password)));
        var result = await response.Content.ReadFromJsonAsync<AccessTokenCreated>();

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

        return await _webApiClient.SendAsync(requestMessage);
    }
}
