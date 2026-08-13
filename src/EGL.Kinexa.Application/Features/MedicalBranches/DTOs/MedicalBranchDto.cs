namespace EGL.Kinexa.Application.Features.MedicalBranches.DTOs;

public record MedicalBranchDto(
    int Id,
    string Name,
    string Slug,
    string Description,
    string? IconUrl,
    bool IsActive,
    int SortOrder,
    int ProductCount
);
