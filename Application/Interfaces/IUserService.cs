using Application.Dto;
using Domain.Enums;
using Domain.Models;

namespace Application.Interfaces;

public interface IUserService
{
    User? Authorize(AuthDto authDto);
    User Register(RegisterDto registerDto);
    User? PutUser(int id, PutUserDto putUserDto);
    User? DeleteUser(int id);
    List<User> GetUsersByDateRange(DateOnly fromDate, DateOnly toDate);
    
    DateOnly? GetMinRegistrationDate();
    DateOnly? GetMaxRegistrationDate();
    IEnumerable<User> GetSortedUsers(Func<User, object> keySelector, bool ascending = true);
    IEnumerable<User> GetUsersByGender(Gender gender);
    int GetTotalUsersCount();
    
    IEnumerable<User> GetUsersPage(int pageNumber, int pageSize);
    IEnumerable<User> GetUsersRangeByIndex(int startInclusive, int endExclusive);
}