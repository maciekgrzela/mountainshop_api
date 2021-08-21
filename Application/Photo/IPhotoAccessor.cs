using Microsoft.AspNetCore.Http;

namespace Application.Photo
{
    public interface IPhotoAccessor
    {
        PhotoUploadResult AddPhoto(IFormFile file);
    }
}