using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using foodremedy.api.Models.Requests;
using foodremedy.api.Models.Responses;
using foodremedy.database.Models;
using Ingredient = foodremedy.database.Models.Ingredient;
using Tag = foodremedy.database.Models.Tag;
using TagCategory = foodremedy.database.Models.TagCategory;

namespace foodremedy.api.tests.Controllers;

internal sealed class IngredientsControllerTests : ControllerTestFixture
{
    [Test]
    public async Task GetIngredients_UnauthenticatedRequest_RespondsUnauthorised()
    {
        HttpResponseMessage response = await _webApiClient.GetAsync("ingredients");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task GetIngredients_ValidRequest_RespondsOkWithPayload()
    {
        var dbIngredient = DbContext.Ingredient.Add(new Ingredient("Some ingredient"));
        await DbContext.SaveChangesAsync();
        
        var request = new HttpRequestMessage(HttpMethod.Get, "Ingredients");
        var response = await SendAuthenticatedRequestAsync(request);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<IngredientSummary>>();

        result.Count.Should().Be(1);
        result.Total.Should().Be(1);
        result.Results.Should().ContainSingle();
        result.Results.First().Id.Should().Be(dbIngredient.Entity.Id);
        result.Results.First().Description.Should().Be(dbIngredient.Entity.Description);
    }

    [Test]
    public async Task GetIngredient_UnauthenticatedRequest_RespondsUnauthorised()
    {
        HttpResponseMessage response = await _webApiClient.GetAsync($"ingredients/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task GetIngredient_IngredientDoesNotExist_RespondsNotFound()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"ingredients/{Guid.NewGuid()}");
        var response = await SendAuthenticatedRequestAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetIngredient_ValidRequest_RespondsOkWithPayload()
    {
        var tagCategory = DbContext.TagCategory.Add(new TagCategory
        {
            Name = "TestCategory",
        }).Entity;
        var dbIngredient = DbContext.Ingredient.Add(new Ingredient("SomeIngredient")
        {
            Tags = new []
            {
                new Tag
                {
                    Name = "TestTag",
                    TagCategory = tagCategory
                }
            }
        }).Entity;
        await DbContext.SaveChangesAsync();

        var request = new HttpRequestMessage(HttpMethod.Get, $"ingredients/{dbIngredient.Id}");
        var response = await SendAuthenticatedRequestAsync(request);
        var result = await response.Content.ReadFromJsonAsync<foodremedy.api.Models.Responses.Ingredient>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Id.Should().Be(dbIngredient.Id);
        result.Description.Should().Be(dbIngredient.Description);
        result.Tags.Should().HaveSameCount(dbIngredient.Tags);
        result.Tags.First().Key.Should().Be(tagCategory.Name);
        result.Tags.First().Value.Should().HaveSameCount(dbIngredient.Tags.Where(p => p.TagCategory.Name == tagCategory.Name));
        result.Tags.First().Value.First().Should().Be(dbIngredient.Tags.First(p => p.TagCategory.Name == tagCategory.Name).Name);
    }

    [Test]
    public async Task CreateIngredient_UnauthenticatedRequest_RespondsUnauthorised()
    {
        HttpResponseMessage response = await _webApiClient.PostAsync(
            "ingredients",
            JsonContent.Create(new CreateIngredient("Test", null))
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task CreateIngredient_WithTagCategoryThatDoesNotExist_RespondsBadRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "ingredients")
        {
            Content = JsonContent.Create(new CreateIngredient(
                "TestIngredient",
                new Dictionary<string, IEnumerable<string>>
                {
                    {"IDontExist", new []{"TestTag"}}
                }
            ))
        };
        var response = await SendAuthenticatedRequestAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateIngredient_WithTagThatDoesNotExist_RespondsBadRequest()
    {
        DbContext.TagCategory.Add(new TagCategory
        {
            Name = "TestCategory"
        });
        await DbContext.SaveChangesAsync();
        
        var request = new HttpRequestMessage(HttpMethod.Post, "ingredients")
        {
            Content = JsonContent.Create(new CreateIngredient(
                "TestIngredient",
                new Dictionary<string, IEnumerable<string>>
                {
                    {"TestCategory", new []{"IDontExist"}}
                }
            ))
        };
        var response = await SendAuthenticatedRequestAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateIngredient_ValidRequestWithTags_RespondsCreatedWithPayload()
    {
        DbContext.TagCategory.Add(new TagCategory
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
        
        var request = new HttpRequestMessage(HttpMethod.Post, "ingredients")
        {
            Content = JsonContent.Create(new CreateIngredient(
                "TestIngredient",
                new Dictionary<string, IEnumerable<string>>
                {
                    {"TestCategory", new []{"TestTag"}}
                }
            ))
        };
        var response = await SendAuthenticatedRequestAsync(request);
        var result = await response.Content.ReadFromJsonAsync<foodremedy.api.Models.Responses.Ingredient>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Id.Should().NotBeEmpty();
        result.Description.Should().Be("TestIngredient");
        result.Tags.Should().ContainSingle();
        result.Tags.First().Key.Should().Be("TestCategory");
        result.Tags.First().Value.Should().ContainSingle();
        result.Tags.First().Value.First().Should().Be("TestTag");
    }

    [Test]
    public async Task CreateIngredient_ValidRequestNoTags_RespondsCreatedWithPayload()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "ingredients")
        {
            Content = JsonContent.Create(new CreateIngredient(
                "TestIngredient",
                null
            ))
        };
        var response = await SendAuthenticatedRequestAsync(request);
        var result = await response.Content.ReadFromJsonAsync<foodremedy.api.Models.Responses.Ingredient>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Id.Should().NotBeEmpty();
        result.Description.Should().Be("TestIngredient");
        result.Tags.Should().BeEmpty();
    }
}
