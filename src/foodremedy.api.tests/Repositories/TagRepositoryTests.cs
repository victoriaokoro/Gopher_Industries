using FluentAssertions;
using foodremedy.api.Repositories;
using foodremedy.api.tests.Utils;
using foodremedy.database.Models;

namespace foodremedy.api.tests.Repositories;

public class TagRepositoryTests : DatabaseIntegrationTestFixture
{
    private readonly TagCategory _tagCategory = new("TestCategory");

    [Fact]
    public async Task Add_should_add_to_db()
    {
        await RunInScopeAsync(context => new TagRepository(context), async (sut, context) =>
        {
            var tagCategory = context.TagCategory.Add(_tagCategory).Entity;
            
            var tag = new Tag("Some description", tagCategory.Id);
            sut.Add(tag);
            await sut.SaveChangesAsync();

            PaginatedResult<Tag> result = await sut.GetAsync();
            Tag? inserted = result.Results.SingleOrDefault(p => p.Id == tag.Id);

            inserted.Should().NotBeNull();
            inserted!.Name.Should().Be(tag.Name);
            inserted.TagCategory.Should().Be(tag.TagCategory);
        });
    }
}
