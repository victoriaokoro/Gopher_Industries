using FluentAssertions;
using foodremedy.api.Controllers;
using foodremedy.api.Models.Requests;
using foodremedy.api.Repositories;
using foodremedy.api.Utils;
using foodremedy.database.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace foodremedy.api.tests.Controllers;

public class UsersControllerTests
{
    private readonly UsersController _sut;
    private readonly Mock<IUserRepository> _userRepository = new();

    public UsersControllerTests()
    {
        _sut = new UsersController(_userRepository.Object);
    }

    [Fact]
    public async Task RegisterUser_should_return_OkObjectResult()
    {
        IActionResult response = await _sut.RegisterUser(new RegisterUser("", ""));
        response.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task RegisterUser_should_respond_conflict_if_user_with_email_exists()
    {
        var user = new User("testEmail", "", "");
        _userRepository.Setup(p => p.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        IActionResult response = await _sut.RegisterUser(new RegisterUser(user.Email, ""));

        response.Should().BeOfType<ConflictResult>();
    }

    [Fact]
    public async Task RegisterUser_should_add_new_user_to_UserRepository()
    {
        var request = new RegisterUser("someEmail", "somePassword");
        User? userCallback = null;
        _userRepository
            .Setup(p => p.Add(It.Is<User>(q => q.Email.Equals(request.Email))))
            .Callback<User>(p => userCallback = p);

        await _sut.RegisterUser(request);

        userCallback.Should().NotBeNull();
        userCallback!.Email.Should().Be(request.Email);
        userCallback.PasswordHash.Should().Be(StringHasher.Hash(request.Password, userCallback.PasswordSalt));
        userCallback.RefreshTokenId.Should().BeNull();
    }
}
