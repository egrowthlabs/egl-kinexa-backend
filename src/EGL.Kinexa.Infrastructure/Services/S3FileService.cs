using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Application.Settings;
using Microsoft.Extensions.Options;

namespace EGL.Kinexa.Infrastructure.Services;

public class S3FileService : IFileService
{
    private readonly IAmazonS3 _s3Client;
    private readonly AwsSettings _settings;

    public S3FileService(IOptions<AwsSettings> settings)
    {
        _settings = settings.Value;
        var config = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(_settings.Region)
        };
        _s3Client = new AmazonS3Client(_settings.AccessKey, _settings.SecretKey, config);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder = "products")
    {
        var key = $"{folder}/{Guid.NewGuid():N}_{fileName}";
        
        var putRequest = new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType,
        };

        await _s3Client.PutObjectAsync(putRequest);
        return GetFileUrl(key);
    }

    public async Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            var key = ExtractKeyFromUrl(fileUrl);
            if (string.IsNullOrEmpty(key)) return false;

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GetFileUrl(string key)
    {
        return $"https://{_settings.BucketName}.s3.{_settings.Region}.amazonaws.com/{key}";
    }

    private string ExtractKeyFromUrl(string fileUrl)
    {
        var baseUrl = $"https://{_settings.BucketName}.s3.{_settings.Region}.amazonaws.com/";
        return fileUrl.StartsWith(baseUrl) ? fileUrl[baseUrl.Length..] : string.Empty;
    }
}
