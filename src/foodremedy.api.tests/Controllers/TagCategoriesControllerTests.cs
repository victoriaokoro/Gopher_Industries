using System.Net;
using System.Net.Http.Json;
using foodremedy.api.Models.Requests;
using foodremedy.api.Models.Responses;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TagCategory = foodremedy.database.Models.TagCategory;

namespace foodremedy.api.tests.Controllers;

internal sealed class TagCategoriesControllerTests : ControllerTestFixture
{
    [Test]
    public async Task GetCategories_UnauthenticatedRequest_ReturnsUnauthorised()
    {
        HttpResponseMessage response = await _webApiClient.GetAsync("tags/categories");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task GetCategories_ValidRequest_ReturnsTagCategories()
    {
        EntityEntry<TagCategory> dbCategory = DbContext.TagCategory.Add(new TagCategory
        {
            Name = "SomeCategory"
        });
        await DbContext.SaveChangesAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, "tags/categories");

        HttpResponseMessage response = await SendAuthenticatedRequestAsync(request);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<Models.Responses.TagCategory>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Count.Should().Be(1);
        result.Total.Should().Be(1);
        result.Results.Should().ContainSingle();
        result.Results.First().Id.Should().Be(dbCategory.Entity.Id);
        result.Results.First().Name.Should().Be(dbCategory.Entity.Name);
    }

    [Test]
    public async Task CreateTagCategory_UnauthenticatedRequest_ReturnsUnauthorised()
    {
        HttpResponseMessage response = await _webApiClient.PostAsync(
            "tags/categories",
            JsonContent.Create(new CreateTagCategory("SomeCategory"))
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task CreateTagCategory_ValidRequest_ReturnsTagCategory()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "tags/categories")
        {
            Content = JsonContent.Create(new CreateTagCategory("SomeCategory"))
        };

        HttpResponseMessage response = await SendAuthenticatedRequestAsync(request);
        var result = await response.Content.ReadFromJsonAsync<Models.Responses.TagCategory>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("SomeCategory");
    }
}
