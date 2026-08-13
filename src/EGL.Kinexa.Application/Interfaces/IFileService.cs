namespace EGL.Kinexa.Application.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder = "products");
    Task<bool> DeleteFileAsync(string fileUrl);
    string GetFileUrl(string key);
}
