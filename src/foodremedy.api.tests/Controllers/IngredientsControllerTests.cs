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
using Ingredient = foodremedy.database.Models.Ingredient;

namespace foodremedy.api.tests.Controllers;

public class IngredientsControllerTests
{
    private readonly List<Ingredient> _dbIngredients;
    private readonly Mock<IIngredientRepository> _ingredientRepository = new();
    private readonly IngredientsController _sut;

    public IngredientsControllerTests()
    {
        _dbIngredients = new List<Ingredient>
        {
            new("Some ingredient") { Id = Guid.NewGuid() },
            new("Some other ingredient") { Id = Guid.NewGuid() }
        };

        _sut = new IngredientsController(_ingredientRepository.Object);

        _ingredientRepository.Setup(p => p.GetAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new PaginatedResult<Ingredient>(_dbIngredients.Count, _dbIngredients.Count, _dbIngredients));
    }

    [Fact]
    public async Task GetIngredients_should_return_ingredients()
    {
        ActionResult<PaginatedResponse<IngredientSummary>>
            response = await _sut.GetIngredients(new PaginationRequest());
        PaginatedResponse<IngredientSummary>? result = response.Unpack();

        response.Result.Should().BeOfType<OkObjectResult>();
        result!.Results.Should().HaveSameCount(_dbIngredients);
        result.Results.Should().Contain(_dbIngredients.Select(p => p.ToSummaryResponseModel()));
        result.Should().BeOfType<PaginatedResponse<IngredientSummary>>();
    }

    [Fact]
    public async Task GetIngredients_should_pass_pagination_paramaters_to_repository()
    {
        var paginationRequest = new PaginationRequest(123, 321);
        await _sut.GetIngredients(paginationRequest);

        _ingredientRepository.Verify(p => p.GetAsync(paginationRequest.Skip, paginationRequest.Take), Times.Once);
    }

    [Fact]
    public async Task GetIngredient_should_return_Ingredient_if_found()
    {
        var returnIngredient = new Ingredient("Some description") { Id = Guid.NewGuid() };

        _ingredientRepository.Setup(p => p.GetByIdAsync(returnIngredient.Id))
            .ReturnsAsync(returnIngredient);

        ActionResult<Models.Responses.Ingredient> response = await _sut.GetIngredient(returnIngredient.Id);
        Models.Responses.Ingredient? result = response.Unpack();

        _ingredientRepository.Verify(p => p.GetByIdAsync(returnIngredient.Id), Times.Once);
        response.Result.Should().BeOfType<OkObjectResult>();
        result.Should().BeOfType<Models.Responses.Ingredient>();
        result!.Id.Should().Be(returnIngredient.Id);
        result.Description.Should().Be(returnIngredient.Description);
    }

    [Fact]
    public async Task GetIngredient_should_return_404_if_not_found()
    {
        var queryId = Guid.NewGuid();
        _ingredientRepository.Setup(p => p.GetByIdAsync(queryId)).ReturnsAsync(null as Ingredient);

        ActionResult<Models.Responses.Ingredient> response = await _sut.GetIngredient(queryId);

        response.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateIngredient_should_save_ingredient_to_db()
    {
        var request = new CreateIngredient("Some description");
        Ingredient? ingredientCallback = null;

        _ingredientRepository
            .Setup(p => p.Add(It.IsAny<Ingredient>()))
            .Callback<Ingredient>(p => ingredientCallback = p)
            .Returns(request.ToDbModel());

        await _sut.CreateIngredient(request);
        
        _ingredientRepository.Verify(p => p.Add(It.IsAny<Ingredient>()), Times.Once);
        ingredientCallback.Should().NotBeNull();
        ingredientCallback!.Description.Should().Be(request.Description);
    }

    [Fact]
    public async Task CreateIngredient_should_return_Created_response()
    {
        _ingredientRepository.Setup(p => p.Add(It.IsAny<Ingredient>()))
            .Returns<Ingredient>(p => p);
        
        var request = new CreateIngredient("Some description");
        
        var response = await _sut.CreateIngredient(request);
        var createdResult = response.Result as CreatedResult;
        ArgumentNullException.ThrowIfNull(createdResult);
        var objectResult = createdResult.Value as Models.Responses.Ingredient;
        ArgumentNullException.ThrowIfNull(objectResult);

        response.Result.Should().BeOfType<CreatedResult>();
        createdResult.Location.Should().Be($"/ingredients/{objectResult.Id}");
        objectResult.Description.Should().Be(request.Description);
    }
}
