using foodremedy.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace foodremedy.api.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class IngredientsController : ControllerBase
{
    [HttpGet]
    public async Task<PaginatedResult<IngredientSummary>> GetIngredientsAsync([FromQuery] int? skip, [FromQuery] int? take)
    {
        return new PaginatedResult<IngredientSummary>(0, 0, new List<IngredientSummary>());
    }

    [HttpGet("{ingredientId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<Ingredient> GetIngredient([FromRoute] string ingredientId)
    {
        return new Ingredient
        {
            Id = ingredientId
        };
    }
}
