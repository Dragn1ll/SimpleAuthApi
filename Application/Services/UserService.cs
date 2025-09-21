using Application.Dto;
using Application.Interfaces;
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
}