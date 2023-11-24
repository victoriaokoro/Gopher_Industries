using System.Net;
using System.Net.Http.Json;
using foodremedy.api.Models.Requests;
using foodremedy.api.Models.Responses;
using Tag = foodremedy.database.Models.Tag;
using TagCategory = foodremedy.database.Models.TagCategory;

namespace foodremedy.api.tests.Controllers;

internal sealed class TagsControllerTests : ControllerTestFixture
{
    [Test]
    public async Task Get_UnauthenticatedRequest_RespondsUnauthorized()
    {
        HttpResponseMessage response = await _webApiClient.GetAsync("tags");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Get_ValidRequest_RespondsOkWithPayload()
    {
        var category = DbContext.TagCategory.Add(new TagCategory
        {
            Name = "TestCategory",
            Tags = new []
            {
                new Tag
                {
                    Name = "TestTag",
                }
            }
        });
        await DbContext.SaveChangesAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "tags");
        var response = await SendAuthenticatedRequestAsync(request);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<foodremedy.api.Models.Responses.Tag>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Count.Should().Be(1);
        result.Total.Should().Be(1);
        result.Results.Should().ContainSingle();
        result.Results.First().Id.Should().Be(category.Entity.Tags.First().Id);
        result.Results.First().Name.Should().Be(category.Entity.Tags.First().Name);
        result.Results.First().TagCategory.Should().Be(category.Entity.Name);
    }

    [Test]
    public async Task CreateTag_UnauthenticatedRequest_RespondsUnauthorized()
    {
        HttpResponseMessage response = await _webApiClient.PostAsync(
            $"tags/{Guid.NewGuid()}",
            JsonContent.Create(new CreateTag("TagName"))
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task CreateTag_CategoryDoesNotExist_RespondsNotFound()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"tags/{Guid.NewGuid()}")
        {
            Content = JsonContent.Create(new CreateTag("SomeTag"))
        };
        var response = await SendAuthenticatedRequestAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task CreateTag_ValidRequest_RespondsCreatedWithPayload()
    {
        var category = DbContext.TagCategory.Add(new TagCategory
        {
            Name = "TestCategory"
        });
        await DbContext.SaveChangesAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, $"tags/{category.Entity.Id}")
        {
            Content = JsonContent.Create(new CreateTag("TestTag"))
        };
        var response = await SendAuthenticatedRequestAsync(request);
        var result = await response.Content.ReadFromJsonAsync<foodremedy.api.Models.Responses.Tag>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("TestTag");
        result.TagCategory.Should().Be(category.Entity.Name);
    }
}
