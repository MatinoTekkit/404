using Microsoft.AspNetCore.Mvc;
using SportReservation.Models;
using SportReservation.Services;

namespace SportReservation.Controllers;

/// <summary>
/// Reserved for public things
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PublicController(UserService userService) : ControllerBase
{
    // spis na veci typu, Kolik je volnych mistnosti blabla to co muze videt i neprihlaseny
    // momentalne je to spis testovaci controller lol

    /// <summary>
    /// Registers new user
    /// </summary>
    /// <param name="body">dto</param>
    /// <returns>created user</returns>
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto body)
    {
        var user = await userService.Register(body, UserRole.User);
        return Ok(user.ToDto());
    }
}