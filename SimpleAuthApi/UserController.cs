using Application.Dto;
using Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SimpleAuthApi.Requests;

namespace SimpleAuthApi;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, IValidator<AuthRequest> validator) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Authorize([FromBody] AuthRequest authRequest)
    {
        var validResult = validator.Validate(authRequest);
        if (!validResult.IsValid)
            return BadRequest(validResult.Errors);
        
        var user = userService.Authorize(new AuthDto
        {
            Email = authRequest.Email,
            Password = authRequest.Password
        });

        if (user == null) return NotFound();
        
        HttpContext.Session.SetInt32("UserId", user.Id);
        return Ok(user);
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest registerRequest)
    {
        var validResult = validator.Validate(registerRequest);
        if (!validResult.IsValid)
            return BadRequest(validResult.Errors);
        
        var user = userService.Register(new RegisterDto
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password,
            Username = registerRequest.Username,
            CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow)
        });
        
        return Ok(user);
    }

    [HttpPut]
    public IActionResult PutUser([FromBody] PutUserRequest putUserRequest)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Unauthorized();
        
        var user = userService.PutUser(userId.Value, new PutUserDto
        {
            Email = putUserRequest.Email,
            Password = putUserRequest.Password,
            Username = putUserRequest.Username,
            UpdatedDate = DateOnly.FromDateTime(DateTime.UtcNow)
        });
        
        return Ok(user);
    }

    [HttpDelete]
    public IActionResult DeleteUser()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Unauthorized();
        
        var user = userService.DeleteUser(userId.Value);
        
        return Ok(user);
    }

    [HttpGet("by-date-range")]
    public IActionResult GetUsersByDateRange(DateOnly fromDate, DateOnly toDate)
    {
        var users = userService.GetUsersByDateRange(fromDate, toDate);
        
        return Ok(users);
    }
}