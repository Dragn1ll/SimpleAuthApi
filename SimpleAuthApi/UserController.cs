using Application.Dto;
using Application.Interfaces;
using Application.Queues;
using Contracts.RabbitMq;
using Contracts.Requests;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace SimpleAuthApi;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, IValidator<AuthRequest> validator, 
    ILogger<UserController> logger, FileLogQueue fileLog, IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Authorize([FromBody] AuthRequest authRequest)
    {
        var validResult = await validator.ValidateAsync(authRequest);
        if (!validResult.IsValid)
            return BadRequest(validResult.Errors);
        
        var user = await userService.Authorize(new AuthDto
        {
            Email = authRequest.Email,
            Password = authRequest.Password
        });

        if (user == null) 
            return NotFound();
        
        HttpContext.Session.SetInt32("UserId", user.Id);
        return Ok(user);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        await fileLog.EnqueueAsync($"[{DateTime.UtcNow:o}] регистрация началась email={registerRequest.Email}");
        var validResult = await validator.ValidateAsync(registerRequest);
        if (!validResult.IsValid)
        {
            logger.LogWarning("Данные не прошли валидацию. {Email}", registerRequest.Email);
            return BadRequest(validResult.Errors);
        }

        logger.LogInformation("Регистрация начата. {Email}", registerRequest.Email);

        var userId = await userService.Register(new RegisterDto
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password,
            Username = registerRequest.Username,
            Gender = registerRequest.Gender,
            CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow)
        });
        
        await publishEndpoint.Publish(new UserRegistered(
            UserId: userId,
            Email: registerRequest.Email,
            Username: registerRequest.Username,
            CreatedDateUtc: DateOnly.FromDateTime(DateTime.UtcNow)));
        
        logger.LogInformation("Регистрация прошла успешно. {Email} {UserId}", registerRequest.Email, userId);
        await fileLog.EnqueueAsync(
            $"[{DateTime.UtcNow:o}] регистрация выполнена успешно email={registerRequest.Email} userId={userId}");

        return Ok(userId);
    }

    [HttpPut]
    public async Task<IActionResult> PutUser([FromBody] PutUserRequest putUserRequest)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Unauthorized();
        
        var isUpdated = await userService.PutUser(userId.Value, new PutUserDto
        {
            Email = putUserRequest.Email,
            Password = putUserRequest.Password,
            Username = putUserRequest.Username,
            UpdatedDate = DateOnly.FromDateTime(DateTime.UtcNow)
        });
        
        return isUpdated ? Ok() : BadRequest();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Unauthorized();
        
        var isDeleted = await userService.DeleteUser(userId.Value);
        
        return isDeleted ? NoContent() : BadRequest();
    }

    [HttpGet("by-date-range")]
    public async Task<IActionResult> GetUsersByDateRange(DateOnly fromDate, DateOnly toDate)
    {
        var users = await userService.GetUsersByDateRange(fromDate, toDate);
        
        return Ok(users);
    }
    
    [HttpGet("stats/registration-date/min")]
    public async Task<ActionResult> GetMinRegistrationDate()
        => Ok(await userService.GetMinRegistrationDate());
    
    [HttpGet("stats/registration-date/max")]
    public async Task<ActionResult> GetMaxRegistrationDate()
        => Ok(await userService.GetMaxRegistrationDate());
    
    [HttpGet("stats/count")]
    public ActionResult GetTotalUsersCount()
        => Ok(userService.GetTotalUsersCount());
    
    [HttpGet("page")]
    public async Task<ActionResult> GetPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var users = await userService.GetUsersPage(pageNumber, pageSize);
        return Ok(users);
    }

    [HttpGet("range")]
    public async Task<ActionResult> GetRange([FromQuery] int startInclusive, [FromQuery] int endExclusive)
    {
        var users = await userService.GetUsersRangeByIndex(startInclusive, endExclusive);
        return Ok(users);
    }
}