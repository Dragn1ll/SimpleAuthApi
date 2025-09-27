using Application.Dto;
using Application.Interfaces;
using Domain.Enums;
using Domain.Models;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly List<User> _users = [];
    private int _nextId = 1;
    
    public User? Authorize(AuthDto authDto)
    {
        return _users.FirstOrDefault(u => u.Email == authDto.Email && u.Password == authDto.Password);
    }
    
    public User Register(RegisterDto registerDto)
    {
        var user = new User
        {
            Email = registerDto.Email,
            Password = registerDto.Password,
            Username = registerDto.Username,
            Id = _nextId++,
            CreatedDate = registerDto.CreatedDate,
            UpdatedDate = registerDto.CreatedDate
        };
        
        _users.Add(user);
        
        return user;
    }
    
    public User? PutUser(int id, PutUserDto putUserDto)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return null;
        
        user.Email = putUserDto.Email;
        user.Password = putUserDto.Password;
        user.Username = putUserDto.Username;
        user.UpdatedDate = putUserDto.UpdatedDate;
        
        return user;
    }
    
    public User? DeleteUser(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return null;
        
        _users.Remove(user);
        
        return user;
    }

    public List<User> GetUsersByDateRange(DateOnly fromDate, DateOnly toDate)
    {
        return _users.Where(u => u.UpdatedDate >= fromDate && u.UpdatedDate <= toDate).ToList();
    }
    
    public DateOnly? GetMinRegistrationDate()
    {
        if (_users.Count == 0) return null;
        return _users.Min(u => u.CreatedDate);
    }

    public DateOnly? GetMaxRegistrationDate()
    {
        if (_users.Count == 0) return null;
        return _users.Max(u => u.CreatedDate);
    }

    public IEnumerable<User> GetSortedUsers(Func<User, object> keySelector, bool ascending = true)
    {
        return ascending ? _users.OrderBy(keySelector).ToList() : _users.OrderByDescending(keySelector).ToList();
    }

    public IEnumerable<User> GetUsersByGender(Gender gender)
    {
        return _users.Where(u => u.Gender == gender).ToList();
    }

    public int GetTotalUsersCount()
    {
        return _users.Count;
    }

    public IEnumerable<User> GetUsersPage(int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var skip = (pageNumber - 1) * pageSize;
        return _users.Skip(skip).Take(pageSize).ToList();
    }

    public IEnumerable<User> GetUsersRangeByIndex(int startInclusive, int endExclusive)
    {
        if (startInclusive < 0) startInclusive = 0;
        if (endExclusive <= startInclusive) return [];

        var count = endExclusive - startInclusive;
        return _users.Skip(startInclusive).Take(count).ToList();
    }
}