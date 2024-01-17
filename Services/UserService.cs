using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Models;
using TodoApi.Services.Interface;

namespace TodoApi.Services;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    public UserService(UserManager<IdentityUser> userManager, IConfiguration conf)
    {
        _userManager = userManager;
        _configuration = conf;
    }
    public async Task<UserManagerResponse> RegisterUserAsync(RegisterModel model)
    {
        try
        {
            if (model == null)
            {
                throw new NullReferenceException("Register model is null");
            }
            if (model.Password != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    Message = "Passwords doesnt matches",
                    IsSuccess = false,
                };

            var identityUser = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                // var confirmedEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                // var encodeEmailToken = Encoding.UTF8.GetBytes(confirmedEmailToken);
                // var validEmailToken = WebEncoders.Base64UrlEncode(encodeEmailToken);

                return new UserManagerResponse
                {
                    Message = "User created successfully",
                    IsSuccess = true
                };
            }

            return new UserManagerResponse
            {
                Message = "User did not created",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<UserManagerResponse> LoginAsync(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return new UserManagerResponse
            {
                Message = "User not found",
                IsSuccess = false,
            };
        }

        // Validate the user's password
        var validPassword = await _userManager.CheckPasswordAsync(user, model.Password);

        if (!validPassword)
        {
            return new UserManagerResponse
            {
                Message = "Invalid password",
                IsSuccess = false,
            };
        }

        var claims = new[]
        {
            new Claim("Email", model.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

        var apiKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:ApiKey"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["Authentication:Issuer"],
            audience: _configuration["Authentication:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(15),
            signingCredentials: new SigningCredentials(apiKey, SecurityAlgorithms.HmacSha256)
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new UserManagerResponse
        {
            Message = accessToken,
            IsSuccess = true,
            ExpirationDate = token.ValidTo
        };
    }
}