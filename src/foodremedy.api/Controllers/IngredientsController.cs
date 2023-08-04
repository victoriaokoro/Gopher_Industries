using foodremedy.api.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace foodremedy.api.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class IngredientsController : ControllerBase
{
    [HttpGet]
    public Task<PaginatedResult<IngredientSummary>> GetIngredientsAsync([FromQuery] int? skip, [FromQuery] int? take)
    {
        return Task.FromResult(new PaginatedResult<IngredientSummary>(0, 0, new List<IngredientSummary>()));
    }

    [HttpGet("{ingredientId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<Ingredient> GetIngredient([FromRoute] string ingredientId)
    {
        return Task.FromResult(new Ingredient(ingredientId, null!,null!));
    }
}
