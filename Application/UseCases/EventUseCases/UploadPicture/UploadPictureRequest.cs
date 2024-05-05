using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.EventUseCases.UploadPicture;

public sealed record UploadPictureRequest(IFormFile file, string eventId): IRequest<UploadPictureResponse>
{
    
}