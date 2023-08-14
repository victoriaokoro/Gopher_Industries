using FluentAssertions;
using foodremedy.api.Controllers;
using foodremedy.api.Extensions;
using foodremedy.api.Models.Requests;
using foodremedy.api.Repositories;
using foodremedy.api.tests.Extensions;
using foodremedy.database.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace foodremedy.api.tests.Controllers;

public class TagsControllerTests
{
    private readonly TagsController _sut;
    private readonly Mock<ITagRepository> _tagRepository = new();

    public TagsControllerTests()
    {
        _sut = new TagsController(_tagRepository.Object);
    }

    [Fact]
    public async Task GetAll_should_return_tags()
    {
        var dbTags = new List<Tag>
        {
            new("tag1", TagType.BENEFIT) { Id = Guid.NewGuid() },
            new("tag2", TagType.PLANT_TYPE) { Id = Guid.NewGuid() }
        };

        _tagRepository
            .Setup(p => p.GetAllAsync())
            .ReturnsAsync(dbTags);

        ActionResult<IEnumerable<Models.Responses.Tag>> response = await _sut.GetAll();
        IEnumerable<Models.Responses.Tag>? result = response.Unpack();

        response.Result.Should().BeOfType<OkObjectResult>();
        result.Should().HaveSameCount(dbTags);
        result.Should().Contain(dbTags.Select(p => p.ToResponseModel()));
    }

    [Fact]
    public async Task CreateTag_should_save_tag_to_repository()
    {
        var request = new CreateTag("Some description", TagType.BENEFIT.ToString());
        Tag? tagCallback = null;
        _tagRepository
            .Setup(p => p.Add(It.IsAny<Tag>()))
            .Callback<Tag>(q => tagCallback = q);

        await _sut.CreateTag(request);

        _tagRepository.Verify(p => p.Add(It.IsAny<Tag>()), Times.Once);
        tagCallback.Should().NotBeNull();
        tagCallback!.TagType.Should().Be(TagType.BENEFIT);
        tagCallback!.Description.Should().Be(request.Description);
    }
}
