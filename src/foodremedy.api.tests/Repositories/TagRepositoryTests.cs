using FluentAssertions;
using foodremedy.api.Repositories;
using foodremedy.api.tests.Utils;
using foodremedy.database.Models;

namespace foodremedy.api.tests.Repositories;

public class TagRepositoryTests : DatabaseIntegrationTestFixture
{
    [Fact]
    public async Task Add_should_add_to_db()
    {
        await RunInScopeAsync(context => new TagRepository(context), async sut =>
        {
            var tag = new Tag("Some description", TagType.BENEFIT);
            sut.Add(tag);
            await sut.SaveChangesAsync();

            PaginatedResult<Tag> result = await sut.GetAsync();
            Tag? inserted = result.Results.SingleOrDefault(p => p.Id == tag.Id);

            inserted.Should().NotBeNull();
            inserted!.Description.Should().Be(tag.Description);
            inserted.TagType.Should().Be(tag.TagType);
        });
    }
}
