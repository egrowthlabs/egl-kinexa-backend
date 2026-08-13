namespace EGL.Kinexa.Application.Features.Products.DTOs;

public record ProductDto(
    int Id,
    string Name,
    string Slug,
    string Description,
    int CategoryId,
    string CategoryName,
    int MedicalBranchId,
    string MedicalBranchName,
    string? ImageUrl,
    bool IsActive,
    int SortOrder
);
