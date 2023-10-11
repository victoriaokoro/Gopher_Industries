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
using Tag = foodremedy.database.Models.Tag;
using TagCategory = foodremedy.database.Models.TagCategory;

namespace foodremedy.api.tests.Controllers;

public class TagsControllerTests
{
    private readonly List<Tag> _dbTags;
    private readonly TagsController _sut;
    private readonly Mock<ITagRepository> _tagRepository = new();
    private readonly Mock<ITagCategoryRepository> _tagCategoryRepository = new();
    private readonly TagCategory _testCategory;

    public TagsControllerTests()
    {
        _testCategory = new TagCategory("Category") { Id = Guid.NewGuid()};
        _dbTags = new List<Tag>
        {
            new("tag1", Guid.NewGuid()) { Id = Guid.NewGuid() },
            new("tag2", Guid.NewGuid()) { Id = Guid.NewGuid() }
        };

        _sut = new TagsController(_tagRepository.Object, _tagCategoryRepository.Object);

        _tagRepository
            .Setup(p => p.GetAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new PaginatedResult<Tag>(_dbTags.Count, _dbTags.Count, _dbTags));
        _tagRepository
            .Setup(p => p.Add(It.IsAny<Tag>()))
            .Returns<Tag>(p => new Tag(p.Name, p.TagCategoryId) { Id = p.Id });
        
        _tagCategoryRepository
            .Setup(p => p.GetByIdAsync(_testCategory.Id))
            .ReturnsAsync(_testCategory);
    }

    [Fact]
    public async Task Get_should_return_tags()
    {
        ActionResult<PaginatedResponse<Models.Responses.Tag>> response = await _sut.Get(new PaginationRequest());
        PaginatedResponse<Models.Responses.Tag>? result = response.Unpack();

        response.Result.Should().BeOfType<OkObjectResult>();
        result!.Results.Should().HaveSameCount(_dbTags);
        result.Results.Should().Contain(_dbTags.Select(p => p.ToResponseModel()));
    }

    [Fact]
    public async Task Get_should_pass_pagination_params_to_repository()
    {
        var paginationRequest = new PaginationRequest(123, 321);
        await _sut.Get(paginationRequest);

        _tagRepository.Verify(p => p.GetAsync(paginationRequest.Skip, paginationRequest.Take), Times.Once);
    }

    [Fact]
    public async Task CreateTag_should_save_tag_to_repository()
    {
        var request = new CreateTag("Some description");
        Tag? tagCallback = null;
        _tagRepository
            .Setup(p => p.Add(It.IsAny<Tag>()))
            .Callback<Tag>(q => tagCallback = q)
            .Returns(request.ToDbModel(_testCategory.Id));

        await _sut.CreateTag(request, _testCategory.Id);

        _tagRepository.Verify(p => p.Add(It.IsAny<Tag>()), Times.Once);
        tagCallback.Should().NotBeNull();
        tagCallback!.TagCategoryId.Should().Be(_testCategory.Id);
        tagCallback!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task CreateTag_should_return_Created_response()
    {
        var request = new CreateTag("Some description");
        _tagRepository
            .Setup(p => p.Add(It.IsAny<Tag>()))
            .Returns(request.ToDbModel(_testCategory.Id));

        ActionResult<Models.Responses.Tag> response = await _sut.CreateTag(request, _testCategory.Id);
        var createdResult = response.Result as CreatedResult;
        var objectResult = createdResult?.Value as Models.Responses.Tag;
        
        _tagRepository.Verify(p => p.SaveChangesAsync(), Times.Once);

        createdResult.Should().NotBeNull();
        objectResult.Should().NotBeNull();
        createdResult!.Location.Should().Be($"/tags/{objectResult!.Id}");
        objectResult.TagCategoryId.Should().Be(_testCategory.Id);
        objectResult.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task CreateTag_should_return_NotFound_if_category_does_not_exist()
    {
        var tagCategoryId = Guid.NewGuid();
        _tagCategoryRepository
            .Setup(p => p.GetByIdAsync(tagCategoryId))
            .ReturnsAsync(null as TagCategory);

        var result = await _sut.CreateTag(new CreateTag("SomeTag"), tagCategoryId);

        result.Result.Should().BeOfType<NotFoundResult>();
    }
}
