using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Users;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]

    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.CreateAsync(dto);

        if (!result.IsSuccess)
            return StatusCode(result.ErrorStatusCode, new { error = result.Error });

        return CreatedAtAction(nameof(GetUsers), new { id = result.Value!.Id }, result.Value);
    }

    [HttpGet]

    public async Task<IActionResult> GetUsers()
    {
        var result = await _userService.GetAllAsync();
        return Ok(result.Value);
    }

    [HttpPut("{id}")]

    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.UpdateAsync(id, dto);

        if (!result.IsSuccess)
            return StatusCode(result.ErrorStatusCode, new { error = result.Error });

        return Ok(result.Value);
    }
}
