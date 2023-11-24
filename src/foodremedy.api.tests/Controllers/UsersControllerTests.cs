using System.Net;
using System.Net.Http.Json;
using foodremedy.api.Models.Requests;

namespace foodremedy.api.tests.Controllers;

internal sealed class UsersControllerTests : ControllerTestFixture
{
    [Test]
    public async Task RegisterUser_ValidRequest_ReturnsOk()
    {
        var request = new RegisterUser("test@test.com", "password");

        HttpResponseMessage result = await _webApiClient.PostAsync(
            "users/register",
            JsonContent.Create(request)
        );

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task RegisterUser_CreatingUserWithDuplicateEmail_ReturnsConflict()
    {
        var request = new RegisterUser("test@test.com", "password");

        HttpResponseMessage initialResult = await _webApiClient.PostAsync(
            "users/register",
            JsonContent.Create(request)
        );
        HttpResponseMessage invalidResult = await _webApiClient.PostAsync(
            "users/register",
            JsonContent.Create(request)
        );

        initialResult.StatusCode.Should().Be(HttpStatusCode.OK);
        invalidResult.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
