using System.Net;
using foodremedy.api.Extensions;
using foodremedy.api.Models.Requests;
using foodremedy.api.Models.Responses;
using foodremedy.api.Repositories;
using foodremedy.database.Models;
using Microsoft.AspNetCore.Mvc;
using Ingredient = foodremedy.api.Models.Responses.Ingredient;
using TagCategory = foodremedy.database.Models.TagCategory;

namespace foodremedy.api.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ITagCategoryRepository _tagCategoryRepository;

    public IngredientsController(IIngredientRepository ingredientRepository,
        ITagCategoryRepository tagCategoryRepository)
    {
        _ingredientRepository = ingredientRepository;
        _tagCategoryRepository = tagCategoryRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<IngredientSummary>>> GetIngredients(
        [FromQuery] PaginationRequest paginationRequest)
    {
        PaginatedResult<database.Models.Ingredient> results =
            await _ingredientRepository.GetAsync(paginationRequest.Skip, paginationRequest.Take);

        return Ok(results.ToResponseModel(p => p.ToSummaryResponseModel()));
    }

    [HttpGet("{ingredientId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Ingredient>> GetIngredient([FromRoute] Guid ingredientId)
    {
        database.Models.Ingredient? result = await _ingredientRepository.GetByIdAsync(ingredientId);

        if (result == null)
            return NotFound();

        return Ok(result.ToResponseModel());
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Ingredient>> CreateIngredient([FromBody] CreateIngredient createIngredient)
    {
        database.Models.Ingredient? result;

        if (createIngredient.Tags.IsNullOrEmpty())
        {
            result = _ingredientRepository.Add(createIngredient.ToDbModel());
            await _ingredientRepository.SaveChangesAsync();

            return Created($"/ingredients/{result.ToResponseModel().Id}", result.ToResponseModel());
        }

        var tagCategories = new List<TagCategory>();

        foreach (KeyValuePair<string, IEnumerable<string>> tagCategory in createIngredient.Tags!)
        {
            TagCategory? dbCategory = await _tagCategoryRepository.GetByName(tagCategory.Key);

            if (dbCategory == null)
                return BadRequest(new ProblemDetails
                {
                    Title = "Bad Request",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = $"The tag type {tagCategory.Key} is invalid"
                });

            IEnumerable<string> invalidCategories = tagCategory.Value.Except(dbCategory.Tags.Select(p => p.Name));
            if (invalidCategories.Any())
                return BadRequest(new ProblemDetails
                {
                    Title = "Bad Request",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = $"The following tags are invalid: {string.Join(", ", invalidCategories)}"
                });

            tagCategories.Add(dbCategory);
        }

        result = _ingredientRepository.Add(createIngredient.ToDbModel(tagCategories));

        await _ingredientRepository.SaveChangesAsync();

        return Created($"/ingredients/{result.ToResponseModel().Id}", result.ToResponseModel());
    }
}
