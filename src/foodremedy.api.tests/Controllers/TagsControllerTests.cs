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

namespace foodremedy.api.tests.Controllers;

public class TagsControllerTests
{
    private readonly List<Tag> _dbTags;
    private readonly TagsController _sut;
    private readonly Mock<ITagRepository> _tagRepository = new();

    public TagsControllerTests()
    {
        _dbTags = new List<Tag>
        {
            new("tag1", TagType.BENEFIT) { Id = Guid.NewGuid() },
            new("tag2", TagType.PLANT_TYPE) { Id = Guid.NewGuid() }
        };

        _sut = new TagsController(_tagRepository.Object);

        _tagRepository
            .Setup(p => p.GetAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new PaginatedResult<Tag>(_dbTags.Count, _dbTags.Count, _dbTags));
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
        var request = new CreateTag("Some description", TagType.BENEFIT.ToString());
        Tag? tagCallback = null;
        _tagRepository
            .Setup(p => p.Add(It.IsAny<Tag>()))
            .Callback<Tag>(q => tagCallback = q)
            .Returns(request.ToDbModel);

        await _sut.CreateTag(request);

        _tagRepository.Verify(p => p.Add(It.IsAny<Tag>()), Times.Once);
        tagCallback.Should().NotBeNull();
        tagCallback!.TagType.Should().Be(TagType.BENEFIT);
        tagCallback!.Description.Should().Be(request.Description);
    }

    [Fact]
    public async Task CreateTag_should_return_Created_response()
    {
        var request = new CreateTag("Some description", TagType.BENEFIT.ToString());
        _tagRepository
            .Setup(p => p.Add(It.IsAny<Tag>()))
            .Returns(request.ToDbModel);

        ActionResult<Models.Responses.Tag> response = await _sut.CreateTag(request);
        var createdResult = response.Result as CreatedResult;
        var objectResult = createdResult?.Value as Models.Responses.Tag;

        createdResult.Should().NotBeNull();
        objectResult.Should().NotBeNull();
        createdResult!.Location.Should().Be($"/tags/{objectResult!.Id}");
        objectResult.TagType.Should().Be(TagType.BENEFIT.ToString());
        objectResult.Description.Should().Be(request.Description);
    }
}
