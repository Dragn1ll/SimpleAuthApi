using Application.Dto;
using Domain.Enums;
using Domain.Models;

namespace Application.Interfaces;

public interface IUserService
{
    Task<User?> Authorize(AuthDto authDto);
    Task<int> Register(RegisterDto registerDto);
    Task<bool> PutUser(int userId, PutUserDto putUserDto);
    Task<bool> DeleteUser(int userId);
    Task<IEnumerable<User>> GetUsersByDateRange(DateOnly fromDate, DateOnly toDate);
    
    Task<DateOnly?> GetMinRegistrationDate();
    Task<DateOnly?> GetMaxRegistrationDate();
    Task<IEnumerable<User>> GetSortedUsers(Func<User, object> keySelector, bool ascending = true);
    Task<IEnumerable<User>> GetUsersByGender(Gender gender);
    int GetTotalUsersCount();
    
    Task<IEnumerable<User>> GetUsersPage(int pageNumber, int pageSize);
    Task<IEnumerable<User>> GetUsersRangeByIndex(int startInclusive, int endExclusive);
}