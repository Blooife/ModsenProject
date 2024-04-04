using System.Security.Claims;
using Application.Models.Dtos;
using Application.Servicies.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegistrationRequestDto model)
    {
        var response = await _authService.RegisterAsync(model);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto model)
    {
        var response = await _authService.LoginAsync(model);
        return Ok(response);
    }
    
    [HttpPost("assignRole")]
    public async Task<IActionResult> AssignRoleAsync([FromBody] AssignRoleRequestDto model)
    {
        var response = await _authService.AssignRoleAsync(model.Email, model.Role.ToUpper());

        return Ok(response);
    }
    
    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDto refreshToken)
    {
        var response = await _authService.RefreshToken(refreshToken.refreshToken);

        return Ok(response);
    }
    
    [HttpPost("registerOnEvent")]
    [Authorize]
    public async Task<IActionResult> RegisterUserOnEvent([FromBody] EventUserDto model)
    {
        var response = await _authService.RegisterUserOnEvent(model.userId, model.eventId);

        return Ok(response);
    }
    
    [HttpPost("unRegisterOnEvent")]
    [Authorize]
    public async Task<IActionResult> UnRegisterUserOnEvent([FromBody] EventUserDto model)
    {
        var response = await _authService.UnregisterUserOnEvent(model.userId, model.eventId);

        return Ok(response);
    }
    
    [HttpGet("getEvents/{id}")]
    [Authorize]
    public async Task<IActionResult> GetAllUserEvents(string id)
    {
        var response = await _authService.GetAllUserEvents(id);

        return Ok(response);
    }
}