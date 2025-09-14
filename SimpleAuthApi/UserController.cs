using Microsoft.AspNetCore.Mvc;
using SimpleAuthApi.Requests;

namespace SimpleAuthApi;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Authorize([FromBody] AuthRequest authRequest)
    {
        var user = userService.Authorize(authRequest);
        
        return user != null ? Ok(user) : NotFound();
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest registerRequest)
    {
        var user = userService.Register(registerRequest);
        
        return Ok(user);
    }

    [HttpPut("{id}")]
    public IActionResult PutUser([FromRoute] int id, [FromBody] PutUserRequest putUserRequest)
    {
        var user = userService.PutUser(id, putUserRequest);
        
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser([FromRoute] int id)
    {
        var user = userService.DeleteUser(id);
        
        return Ok(user);
    }
}