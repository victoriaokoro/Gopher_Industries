using FluentAssertions;
using foodremedy.api.Repositories;
using foodremedy.api.tests.Utils;
using foodremedy.database.Models;

namespace foodremedy.api.tests.Repositories;

public class UserRepositoryTests : DatabaseIntegrationTestFixture
{
    [Fact]
    public async Task AddUser_should_save_add_user()
    {
        await RunInScopeAsync(context => new UserRepository(context),
            async sut =>
            {
                var user = new User("testEmail", "testPassword", "testSalt");
                sut.Add(user);
                await sut.SaveChangesAsync();

                User? result = await sut.GetByEmailAsync(user.Email);

                result.Should().NotBeNull();
                result!.Email.Should().Be(user.Email);
                result.Id.Should().NotBeEmpty();
                result.PasswordHash.Should().Be(user.PasswordHash);
                result.PasswordSalt.Should().Be(user.PasswordSalt);
            });
    }

    [Fact]
    public async Task GetUserByEmail_should_return_user_if_exists()
    {
        await RunInScopeAsync(context => new UserRepository(context),
            async sut =>
            {
                var user = new User("testEmail", "testPassword", "testSalt");
                
                sut.Add(user);
                await sut.SaveChangesAsync();

                var result = await sut.GetByEmailAsync(user.Email);

                result.Should().NotBeNull();
                result!.Email.Should().Be(user.Email);
                result.Id.Should().NotBeEmpty();
                result.PasswordHash.Should().Be(user.PasswordHash);
                result.PasswordSalt.Should().Be(user.PasswordSalt);
            });
    }

    [Fact]
    public async Task GetUserByEmail_should_return_null_if_not_exists()
    {
        await RunInScopeAsync(context => new UserRepository(context),
            async sut =>
            {
                var result = await sut.GetByEmailAsync("testEmail");

                result.Should().BeNull();
            });
    }
    
    [Fact]
    public async Task GetUserById_should_return_user_if_exists()
    {
        await RunInScopeAsync(context => new UserRepository(context),
            async sut =>
            {
                var user = new User("testEmail", "testPassword", "testSalt");
                
                sut.Add(user);
                await sut.SaveChangesAsync();

                var result = await sut.GetByIdAsync(user.Id.ToString());

                result.Should().NotBeNull();
                result!.Email.Should().Be(user.Email);
                result.Id.Should().NotBeEmpty();
                result.PasswordHash.Should().Be(user.PasswordHash);
                result.PasswordSalt.Should().Be(user.PasswordSalt);
            });
    }

    [Fact]
    public async Task GetUserById_should_return_null_if_not_exists()
    {
        await RunInScopeAsync(context => new UserRepository(context),
            async sut =>
            {
                var result = await sut.GetByIdAsync(Guid.NewGuid().ToString());

                result.Should().BeNull();
            });
    }
    
    [Fact]
    public async Task GetUserById_should_return_null_if_id_invalid()
    {
        await RunInScopeAsync(context => new UserRepository(context),
            async sut =>
            {
                var result = await sut.GetByIdAsync("invalidId");

                result.Should().BeNull();
            });
    }
}
