namespace EGL.Kinexa.Application.Features.Products.DTOs;

public record ProductDetailDto(
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
    int SortOrder,
    string? SeoKeywords,
    string? UsageIndications,
    string? Material,
    string? MaterialType,
    string? Measurements,
    string? SpecificInstruments,
    string? Competitors
);
