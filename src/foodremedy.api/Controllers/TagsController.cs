using foodremedy.api.Extensions;
using foodremedy.api.Models.Requests;
using foodremedy.api.Models.Responses;
using foodremedy.api.Repositories;
using foodremedy.database.Models;
using Microsoft.AspNetCore.Mvc;
using Tag = foodremedy.api.Models.Responses.Tag;

namespace foodremedy.api.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class TagsController : ControllerBase
{
    private readonly ITagRepository _tagRepository;
    private readonly ITagCategoryRepository _tagCategoryRepository;

    public TagsController(ITagRepository tagRepository, ITagCategoryRepository tagCategoryRepository)
    {
        _tagRepository = tagRepository;
        _tagCategoryRepository = tagCategoryRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<Tag>>> Get([FromQuery] PaginationRequest paginationRequest)
    {
        PaginatedResult<database.Models.Tag> results = await _tagRepository.GetAsync(paginationRequest.Skip, paginationRequest.Take);
        return Ok(results.ToResponseModel(p => p.ToResponseModel()));
    }

    [HttpPost("{tagCategoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Tag>> CreateTag([FromBody] CreateTag createTag, [FromRoute] Guid tagCategoryId)
    {
        var tagCategory = await _tagCategoryRepository.GetByIdAsync(tagCategoryId);

        if (tagCategory == null)
            return NotFound();
        
        var result = _tagRepository.Add(createTag.ToDbModel(tagCategory.Id));
        await _tagRepository.SaveChangesAsync();
        
        return Created($"/tags/{result.ToResponseModel().Id}", result.ToResponseModel());
    }
}
