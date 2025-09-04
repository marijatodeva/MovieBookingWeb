using MovieWeb.Models;
using MovieWeb.Models.System;
using MovieWeb.Models.System;

namespace MovieWeb.Services
{
    public interface IUserService
    {
        Task<LoginResponse> CheckUserCredidentals(LoginRequest loginRequest);
        Task<bool> LogoutUser(string username);
        Task<bool> TryRegisterRequest(RegisterRequest registerRequest);
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
    }
}
