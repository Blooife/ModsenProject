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
    public async Task<IActionResult> RegisterAsync([FromBody] RegistrationRequestDto model, CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(model, cancellationToken);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto model, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(model, cancellationToken);
        return Ok(response);
    }
    
    [HttpPost("assignRole")]
    public async Task<IActionResult> AssignRoleAsync([FromBody] AssignRoleRequestDto model, CancellationToken cancellationToken)
    {
        var response = await _authService.AssignRoleAsync(model.Email, model.Role.ToUpper(), cancellationToken);

        return Ok(response);
    }
    
    [HttpPost("createRole")]
    public async Task<IActionResult> CreateRoleAsync([FromBody] string roleName)
    {
        var response = await _authService.CreateRoleAsync(roleName);

        return Ok(response);
    }
    
    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDto refreshToken, CancellationToken cancellationToken)
    {
        var response = await _authService.RefreshToken(refreshToken.refreshToken, cancellationToken);

        return Ok(response);
    }
}