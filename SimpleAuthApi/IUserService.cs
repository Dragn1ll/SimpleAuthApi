using SimpleAuthApi.Requests;

namespace SimpleAuthApi;

public interface IUserService
{
    User? Authorize(AuthRequest authRequest);
    User Register(RegisterRequest registerRequest);
    User? PutUser(int id, PutUserRequest putUserRequest);
    User? DeleteUser(int id);
}