using System.Net;
using System.Net.Http.Json;
using foodremedy.api.Models.Requests;
using foodremedy.api.Models.Responses;
using User = foodremedy.database.Models.User;

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

    [Test]
    public async Task GetUsers_ValidRequest_ReturnsOkWithPayload()
    {
        var user1 = new User("someEmail", "somePasswordHash", "somePasswordSalt");
        var user2 = new User("someEmail2", "somePasswordHash2", "somePasswordSalt2");

        User user1Saved = DbContext.User.Add(user1).Entity;
        User user2Saved = DbContext.User.Add(user2).Entity;
        await DbContext.SaveChangesAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "/users");
        HttpResponseMessage response = await SendAuthenticatedRequestAsync(request);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<Models.Responses.User>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Total.Should().BeGreaterOrEqualTo(2);
        result.Count.Should().BeGreaterThanOrEqualTo(2);
        result.Results.Should()
            .Contain(p => p.Email == user1.Email && p.Id == user1Saved.Id).
            And.Contain(p => p.Email == user2.Email && p.Id == user2Saved.Id);
    }
}
