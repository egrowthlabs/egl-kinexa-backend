namespace EGL.Kinexa.Application.Features.Categories.DTOs;

public record CategoryDto(
    int Id,
    string Name,
    string Slug,
    string Description,
    string? LogoUrl,
    bool IsActive,
    int SortOrder,
    int ProductCount
);
