using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TodoApi.Models;
using TodoApi.Services.Interface;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    public AuthenticationController(IUserService userService)
    {
        _userService = userService;
        // _configuration = configuration;
    }
    //api/auth/register
    [HttpPost("Register")]
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await _userService.RegisterUserAsync(model);

            if (response.IsSuccess)
            {
                return Ok(response); //Status code: 200
            }
            return BadRequest(response);
        }
        return BadRequest("Invalid properties");
    }

    [HttpPost("Login")]

    public async Task<ActionResult> LoginAsync([FromBody] LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await _userService.LoginAsync(model);

            if (response.IsSuccess)
            {
                var __response = new ObjectResult(response)
                {
                    StatusCode = 200,
                    Value = new
                    {
                        Message = "login successfuly",
                        Data = response
                    },
                };
                return __response;
            }

            var _response = new ObjectResult(response)
            {
                StatusCode = 400,
                Value = new
                {
                    Message = "nao foi possivel fazer login",
                    Data = response
                },
            };
            return _response;
        }
        return BadRequest("invalid properties");
    }

}