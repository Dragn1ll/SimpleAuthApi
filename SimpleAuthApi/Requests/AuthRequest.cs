using System.ComponentModel.DataAnnotations;

namespace SimpleAuthApi.Requests;

public record AuthRequest(
    [Required] string Email,
    [Required] string Password
    );