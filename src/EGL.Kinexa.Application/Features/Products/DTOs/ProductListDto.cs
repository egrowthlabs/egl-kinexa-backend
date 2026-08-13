namespace EGL.Kinexa.Application.Features.Products.DTOs;

public record ProductListDto(
    int Id,
    string Name,
    string Slug,
    string CategoryName,
    string MedicalBranchName,
    string? ImageUrl,
    bool IsActive
);
