namespace Domain.Models;

public class UserProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
}