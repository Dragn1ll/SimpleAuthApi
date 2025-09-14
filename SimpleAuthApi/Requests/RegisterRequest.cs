using System.ComponentModel.DataAnnotations;

namespace SimpleAuthApi.Requests;

public record RegisterRequest(
    [Required] string Email,
    [Required] string Password,
    [Required] string Username
    );