using FluentAssertions;
using foodremedy.api.Extensions;
using foodremedy.api.Models.Requests;
using foodremedy.database.Models;

namespace foodremedy.api.tests.Extensions;

public class DatabaseModelExtensionsTests
{
    [Fact]
    public void Should_map_between_CreateTag_and_Tag()
    {
        var createTag = new CreateTag("Some description", TagType.MINERAL.ToString());

        var result = createTag.ToDbModel();

        result.Description.Should().Be(createTag.Description);
        result.TagType.Should().Be(TagType.MINERAL);
    }
    
    [Fact]
    public void Should_throw_on_map_between_CreateTag_and_Tag_invalid_TagType()
    {
        var createTag = new CreateTag("Some description", "ThisShouldThrow");

        var act = () => createTag.ToDbModel();

        act.Should().Throw<ArgumentException>().WithMessage("Invalid tag type: ThisShouldThrow*").WithParameterName("TagType");
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
