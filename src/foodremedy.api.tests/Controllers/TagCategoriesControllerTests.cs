using FluentAssertions;
using foodremedy.api.Controllers;
using foodremedy.api.Extensions;
using foodremedy.api.Models.Requests;
using foodremedy.api.Models.Responses;
using foodremedy.api.Repositories;
using foodremedy.api.tests.Extensions;
using foodremedy.database.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TagCategory = foodremedy.database.Models.TagCategory;

namespace foodremedy.api.tests.Controllers;

public class TagCategoriesControllerTests
{
    private readonly TagCategoriesController _sut;
    private readonly Mock<ITagCategoryRepository> _tagCategoryRepository = new();
    private readonly List<TagCategory> _testDbResult;

    public TagCategoriesControllerTests()
    {
        _testDbResult = new List<TagCategory>
        {
            new("category1") { Id = Guid.NewGuid() },
            new("category2") { Id = Guid.NewGuid() },
            new("category3") { Id = Guid.NewGuid() }
        };

        _tagCategoryRepository
            .Setup(p => p.Add(It.IsAny<TagCategory>()))
            .Returns<TagCategory>(p => new TagCategory(p.Name) {Id = Guid.NewGuid()});
        
        _sut = new TagCategoriesController(_tagCategoryRepository.Object);
    }

    [Fact]
    public async Task GetCategories_should_return_paginated_results()
    {
        var req = new PaginationRequest(1, 2);
        var dbResult = new PaginatedResult<TagCategory>(2, _testDbResult.Count,
            _testDbResult.Skip(req.Skip).Take(req.Take).ToList());

        _tagCategoryRepository
            .Setup(p => p.GetAsync(req.Skip, req.Take))
            .ReturnsAsync(dbResult);

        ActionResult<PaginatedResponse<Models.Responses.TagCategory>> response = await _sut.GetCategories(req);
        PaginatedResponse<Models.Responses.TagCategory>? result = response.Unpack();

        _tagCategoryRepository.Verify(p => p.GetAsync(req.Skip, req.Take), Times.Once);

        response.Result.Should().BeOfType<OkObjectResult>();
        result.Should().BeOfType<PaginatedResponse<Models.Responses.TagCategory>>();
        result!.Count.Should().Be(req.Take);
        result.Results.Should().Contain(_testDbResult.Skip(req.Skip).Take(req.Take).Select(p => p.ToResponseModel()));
        result.Total.Should().Be(_testDbResult.Count);
    }

    [Fact]
    public async Task CreateTagCategory_should_add_tag_to_db()
    {
        var request = new CreateTagCategory("Some category");
        var response = await _sut.CreateTagCategory(request);
        
        var createdResult = response.Result as CreatedResult;
        var objectResult = createdResult?.Value as Models.Responses.TagCategory;

        response.Result.Should().BeOfType<CreatedResult>();
        objectResult.Name.Should().Be(request.Name);
    }
}
