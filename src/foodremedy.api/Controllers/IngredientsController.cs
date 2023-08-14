using foodremedy.api.Extensions;
using foodremedy.api.Models.Requests;
using foodremedy.api.Models.Responses;
using foodremedy.api.Repositories;
using foodremedy.database.Models;
using Microsoft.AspNetCore.Mvc;
using Ingredient = foodremedy.api.Models.Responses.Ingredient;

namespace foodremedy.api.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientRepository _ingredientRepository;

    public IngredientsController(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
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
        database.Models.Ingredient result = _ingredientRepository.Add(createIngredient.ToDbModel());
        await _ingredientRepository.SaveChangesAsync();

        return Created($"/ingredients/{result.ToResponseModel().Id}", result.ToResponseModel());
    }
}
