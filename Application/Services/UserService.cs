using Application.Dto;
using Application.Interfaces;
using Domain.Enums;
using Domain.Models;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<User?> Authorize(AuthDto authDto)
    {
        return await _repository.GetByFilter(u => u.Email == authDto.Email && u.Password == authDto.Password);
    }
    
    public async Task<int> Register(RegisterDto registerDto)
    {
        var user = new User
        {
            Email = registerDto.Email,
            Password = registerDto.Password,
            Username = registerDto.Username,
            CreatedDate = registerDto.CreatedDate,
            UpdatedDate = registerDto.CreatedDate
        };
        
        return await _repository.Add(user);
    }
    
    public async Task<bool> PutUser(int userId, PutUserDto putUserDto)
    {
        try
        {
            await _repository.Update(userId, user =>
            {
                user.Email = putUserDto.Email;
                user.Password = putUserDto.Password;
                user.Username = putUserDto.Username;
                user.UpdatedDate = putUserDto.UpdatedDate;
            });
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
    
    public async Task<bool> DeleteUser(int userId)
    {
        try
        {
            await _repository.Remove(userId);
        
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public Task<IEnumerable<User>> GetUsersByDateRange(DateOnly fromDate, DateOnly toDate)
    {
        return _repository.GetAllByFilter(u => u.UpdatedDate >= fromDate && u.UpdatedDate <= toDate);
    }
    
    public async Task<DateOnly?> GetMinRegistrationDate()
    {
        return await _repository.GetMinRegistrationDate();
    }

    public async Task<DateOnly?> GetMaxRegistrationDate()
    {
        return await _repository.GetMaxRegistrationDate();
    }

    public async Task<IEnumerable<User>> GetSortedUsers(Func<User, object> keySelector, bool ascending = true)
    {
        var users = await _repository.GetAllByFilter(_ => true);
        return ascending 
            ? users.OrderBy(keySelector).ToList() 
            : users.OrderByDescending(keySelector).ToList();
    }

    public async Task<IEnumerable<User>> GetUsersByGender(Gender gender)
    {
        return await _repository.GetAllByFilter(u => u.Gender.Id == (int)gender);
    }

    public int GetTotalUsersCount()
    {
        return _repository.UsersCount;
    }

    public async Task<IEnumerable<User>> GetUsersPage(int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var skip = (pageNumber - 1) * pageSize;
        return await _repository.GetAllByRange(skip, pageSize);
    }

    public async Task<IEnumerable<User>> GetUsersRangeByIndex(int startInclusive, int endExclusive)
    {
        if (startInclusive < 0) startInclusive = 0;
        if (endExclusive <= startInclusive) return [];

        var count = endExclusive - startInclusive;
        return await _repository.GetAllByRange(startInclusive, count);
    }
}