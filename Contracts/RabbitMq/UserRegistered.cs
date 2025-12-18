namespace Contracts.RabbitMq;

public sealed record UserRegistered(
    int UserId,
    string Email,
    string Username,
    DateOnly CreatedDateUtc);