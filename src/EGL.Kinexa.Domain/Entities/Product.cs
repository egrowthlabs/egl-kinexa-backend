using EGL.Kinexa.Domain.Common;

namespace EGL.Kinexa.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int MedicalBranchId { get; set; }
    public string? SeoKeywords { get; set; }
    public string? UsageIndications { get; set; }
    public string? Material { get; set; }
    public string? MaterialType { get; set; }
    public string? Measurements { get; set; }
    public string? SpecificInstruments { get; set; }
    public string? Competitors { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
    
    public Category Category { get; set; } = null!;
    public MedicalBranch MedicalBranch { get; set; } = null!;
}
