using System.Collections.Generic;
using EGL.Kinexa.Domain.Common;

namespace EGL.Kinexa.Domain.Entities;

public class MedicalBranch : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
