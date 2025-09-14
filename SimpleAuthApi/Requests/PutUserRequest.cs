using System.ComponentModel.DataAnnotations;

namespace SimpleAuthApi.Requests;

public record PutUserRequest(
    [Required] string Email,
    [Required] string Password,
    [Required] string Username
    );