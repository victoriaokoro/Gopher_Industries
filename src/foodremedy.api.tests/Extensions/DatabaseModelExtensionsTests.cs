using FluentAssertions;
using foodremedy.api.Extensions;
using foodremedy.api.Models.Requests;

namespace foodremedy.api.tests.Extensions;

public class DatabaseModelExtensionsTests
{
    [Fact]
    public void Should_map_between_CreateTag_and_Tag()
    {
        var tagCategoryId = Guid.NewGuid();
        var createTag = new CreateTag("Some description");

        var result = createTag.ToDbModel(tagCategoryId);

        result.Name.Should().Be(createTag.Name);
        result.TagCategoryId.Should().Be(tagCategoryId);
    }

    [Fact]
    public void SHould_map_between_RegisterUser_and_User()
    {
        var registerUser = new RegisterUser("someEmail", "somePassword");

        var result = registerUser.ToDbModel();

        result.Email.Should().Be(registerUser.Email);
        result.PasswordHash.Should().NotBeNullOrWhiteSpace();
        result.PasswordHash.Should().NotBe(registerUser.Password);
        result.PasswordSalt.Should().NotBeNullOrWhiteSpace();
    }
}
