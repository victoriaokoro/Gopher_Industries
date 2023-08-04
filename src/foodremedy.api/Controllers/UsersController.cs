using foodremedy.api.Models.Requests;
using foodremedy.api.Repositories;
using foodremedy.api.Utils;
using foodremedy.database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodremedy.api.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status200OK)]
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
        User? existingUser = await _userRepository.GetUserByEmailAsync(registerUser.Email);

        if (existingUser != null)
            return Conflict();

        string salt = StringHasher.GenerateSalt();

        _userRepository.AddUser(new User(registerUser.Email, StringHasher.Hash(registerUser.Password, salt), salt));
        await _userRepository.SaveChangesAsync();

        return Ok();
    }
}
