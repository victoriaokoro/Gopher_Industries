using foodremedy.api.Extensions;
using foodremedy.api.Models.Requests;
using foodremedy.api.Repositories;
using foodremedy.database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodremedy.api.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUser registerUser)
    {
        User? existingUser = await _userRepository.GetByEmailAsync(registerUser.Email);

        if (existingUser != null)
            return Conflict();

        _userRepository.Add(registerUser.ToDbModel());
        await _userRepository.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<Models.Responses.PaginatedResponse<User>>> GetUsers([FromQuery] PaginationRequest paginationRequest)
    {
        var results = await _userRepository.GetAsync(paginationRequest.Skip, paginationRequest.Take);

        return Ok(results.ToResponseModel(p => p.ToResponseModel()));
    }
}
