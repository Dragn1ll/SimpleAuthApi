using Domain.Enums;

namespace Domain.Models;

public class User
{
    public int Id { get; init; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateOnly CreatedDate { get; init; }
    public DateOnly UpdatedDate { get; set; }
    public Gender Gender { get; set; } = Gender.Other;
    public Role Role { get; set; } = Role.User;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginDate { get; set; }
}