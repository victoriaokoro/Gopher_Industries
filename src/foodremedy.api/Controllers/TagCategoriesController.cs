using foodremedy.api.Extensions;
using foodremedy.api.Models.Requests;
using foodremedy.api.Models.Responses;
using foodremedy.api.Repositories;
using foodremedy.database.Models;
using Microsoft.AspNetCore.Mvc;
using TagCategory = foodremedy.api.Models.Responses.TagCategory;

namespace foodremedy.api.Controllers;

[ApiController]
[Route("tags/categories")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class TagCategoriesController : ControllerBase
{
    private readonly ITagCategoryRepository _tagCategoryRepository;

    public TagCategoriesController(ITagCategoryRepository tagCategoryRepository)
    {
        _tagCategoryRepository = tagCategoryRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<TagCategory>>> GetCategories([FromQuery] PaginationRequest paginationRequest)
    {
        PaginatedResult<database.Models.TagCategory> results = await _tagCategoryRepository.GetAsync(paginationRequest.Skip, paginationRequest.Take);

        return Ok(results.ToResponseModel(p => p.ToResponseModel()));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TagCategory>> CreateTagCategory(CreateTagCategory createTagCategory)
    {
        database.Models.TagCategory tagCategory = _tagCategoryRepository.Add(createTagCategory.ToDbModel());
        await _tagCategoryRepository.SaveChangesAsync();

        return Created($"tags/categories/{tagCategory.ToResponseModel().Id}", tagCategory.ToResponseModel());
    }
}
