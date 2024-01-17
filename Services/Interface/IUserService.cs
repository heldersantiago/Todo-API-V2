using TodoApi.Models;

namespace TodoApi.Services.Interface;

public interface IUserService
{
    Task<UserManagerResponse> RegisterUserAsync(RegisterModel registerModel);
    Task<UserManagerResponse> LoginAsync(LoginModel model);
}