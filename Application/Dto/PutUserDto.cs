namespace Application.Dto;

public class PutUserDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Username { get; init; }
    public required DateOnly UpdatedDate { get; set; }
}