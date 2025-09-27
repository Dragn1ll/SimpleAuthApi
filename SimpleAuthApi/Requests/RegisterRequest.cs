using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace SimpleAuthApi.Requests;

public record RegisterRequest(
    [Required] string Email,
    [Required] string Password,
    [Required] string Username, 
    [Required] Gender Gender
    ) : AuthRequest(Email, Password);