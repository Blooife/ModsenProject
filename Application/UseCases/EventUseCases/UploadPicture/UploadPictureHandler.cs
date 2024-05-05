using Application.Exceptions;
using AutoMapper;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.UseCases.EventUseCases.UploadPicture;

public class UploadPictureHandler: IRequestHandler<UploadPictureRequest, UploadPictureResponse>
{

    public UploadPictureHandler()
    {
    }
    public async Task<UploadPictureResponse> Handle(UploadPictureRequest request, CancellationToken cancellationToken)
    {
        var fileExtension = Path.GetExtension(request.file.FileName);

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", };

        if (allowedExtensions.Contains(fileExtension.ToLowerInvariant()))
        {
            var parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            var path = Path.Combine(parentDirectory,"frontapp/public/pictures", request.eventId+request.file.FileName);

            using var stream = new FileStream(path, FileMode.Create);
            await request.file.CopyToAsync(stream, cancellationToken);
        }
        else
        {
            throw new ImageUploadException("Wrong extension");
        }
        return new UploadPictureResponse() { Message = request.eventId+request.file.FileName};
    }
}