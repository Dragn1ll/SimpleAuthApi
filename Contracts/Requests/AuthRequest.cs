using System.ComponentModel.DataAnnotations;

namespace Contracts.Requests;

public record AuthRequest(
    [Required] string Email,
    [Required] string Password
    );