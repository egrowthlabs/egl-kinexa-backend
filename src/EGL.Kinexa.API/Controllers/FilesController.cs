using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EGL.Kinexa.API.Controllers;

public class FilesController : BaseController
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Administrador,CreadorContenido")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string folder = "products")
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<object>.Fail("No se proporcionó un archivo."));

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            return BadRequest(ApiResponse<object>.Fail("Solo se permiten imágenes (jpg, png, webp, gif)."));

        if (file.Length > 10 * 1024 * 1024) // 10MB
            return BadRequest(ApiResponse<object>.Fail("El archivo no puede superar 10MB."));

        using var stream = file.OpenReadStream();
        var url = await _fileService.UploadFileAsync(stream, file.FileName, file.ContentType, folder);

        return Ok(ApiResponse<object>.Ok(new { url }));
    }

    [HttpDelete]
    [Authorize(Roles = "Administrador,CreadorContenido")]
    public async Task<IActionResult> Delete([FromQuery] string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return BadRequest(ApiResponse<object>.Fail("URL requerida."));

        var result = await _fileService.DeleteFileAsync(url);
        return result 
            ? Ok(ApiResponse<object>.Ok(new { deleted = true }))
            : BadRequest(ApiResponse<object>.Fail("No se pudo eliminar el archivo."));
    }
}
