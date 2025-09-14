using SimpleAuthApi.Requests;

namespace SimpleAuthApi;

public class UserService : IUserService
{
    private readonly List<User> _users = [];
    private int _nextId = 1;
    
    public User? Authorize(AuthRequest authRequest)
    {
        return _users.FirstOrDefault(u => u.Email == authRequest.Email && u.Password == authRequest.Password);
    }
    
    public User Register(RegisterRequest registerRequest)
    {
        var user = new User
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password,
            Username = registerRequest.Username,
            Id = _nextId++
        };
        
        _users.Add(user);
        
        return user;
    }
    
    public User? PutUser(int id, PutUserRequest putUserRequest)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return null;
        
        user.Email = putUserRequest.Email;
        user.Password = putUserRequest.Password;
        user.Username = putUserRequest.Username;
        
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
}