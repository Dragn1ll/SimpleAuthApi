using System.Linq.Expressions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class UserRepository : IUserRepository
{
    public int UsersCount => _context.Users.Count();
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<int> Add(User user)
    {
        var entity = new UserEntity
        {
            Username = user.Username,
            Email = user.Email,
            Password = user.Password,
            UpdatedDate = user.UpdatedDate,
            CreatedDate = user.CreatedDate,
            IsActive = user.IsActive,
            GenderId = (int)user.Gender,
            RoleId = (int)user.Role
        };
        
        _context.Users.Add(entity);
        await _context.SaveChangesAsync();
        
        return entity.Id;
    }

    public async Task<User?> GetByFilter(Expression<Func<UserEntity, bool>> predicate)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(predicate);
        
        return entity == null 
            ? null 
            : ConvertToUser(entity);
    }

    public async Task<IEnumerable<User>> GetAllByFilter(Expression<Func<UserEntity, bool>> predicate)
    {
        var entities = await _context.Users.Where(predicate).ToListAsync();
        
        return entities.Select(ConvertToUser);
    }

    public async Task Update(int userId, Action<UserEntity> updateAction)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        
        updateAction(user);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task Remove(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<DateOnly?> GetMinRegistrationDate()
    {
        var date = await _context.Users.MinAsync(x => x.CreatedDate);
        
        return date;
    }

    public async Task<DateOnly?> GetMaxRegistrationDate()
    {
        var date = await _context.Users.MaxAsync(x => x.CreatedDate);
        
        return date;
    }

    public async Task<IEnumerable<User>> GetAllByRange(int startIndex, int range)
    {
        var entities = await _context.Users.Skip(startIndex).Take(range).ToListAsync();
        
        return entities.Select(ConvertToUser);
    }

    private User ConvertToUser(UserEntity userEntity)
    {
        return new User
        {
            Id = userEntity.Id,
            Username = userEntity.Username,
            Email = userEntity.Email,
            Password = userEntity.Password,
            UpdatedDate = userEntity.UpdatedDate,
            CreatedDate = userEntity.CreatedDate,
            IsActive = userEntity.IsActive,
            Gender = (Gender)userEntity.GenderId,
            LastLoginDate = userEntity.LastLoginDate,
            Role = (Role)userEntity.RoleId
        };
    }
}