using Application.UseCases.AuthUseCases.AssignRole;
using Application.UseCases.AuthUseCases.CreateRole;
using Application.UseCases.AuthUseCases.Login;
using Application.UseCases.AuthUseCases.RefreshToken;
using Application.UseCases.AuthUseCases.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest model, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(model, cancellationToken);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest model, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(model, cancellationToken);
        return Ok(response);
    }
    
    [HttpPost("assignRole")]
    public async Task<IActionResult> AssignRoleAsync([FromBody] AssignRoleRequest model, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(model, cancellationToken);

        return Ok(response);
    }
    
    [HttpPost("createRole")]
    public async Task<IActionResult> CreateRoleAsync([FromBody] string roleName)
    {
        var response = await _mediator.Send(new CreateRoleRequest(roleName));

        return Ok(response);
    }
    
    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshToken, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(refreshToken, cancellationToken);

        return Ok(response);
    }
}