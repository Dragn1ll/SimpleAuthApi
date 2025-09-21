using Application.Dto;
using Domain.Models;

namespace Application.Interfaces;

public interface IUserService
{
    User? Authorize(AuthDto authDto);
    User Register(RegisterDto registerDto);
    User? PutUser(int id, PutUserDto putUserDto);
    User? DeleteUser(int id);
    List<User> GetUsersByDateRange(DateOnly fromDate, DateOnly toDate);
}