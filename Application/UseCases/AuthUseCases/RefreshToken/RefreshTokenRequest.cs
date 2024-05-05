using MediatR;

namespace Application.UseCases.AuthUseCases.RefreshToken;

public sealed record RefreshTokenRequest: IRequest<RefreshTokenResponse>
{
    public string refreshToken { get; set; }
}