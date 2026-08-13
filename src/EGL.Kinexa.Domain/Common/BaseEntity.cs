using System;

namespace EGL.Kinexa.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string? CreatedBy { get; set; }
    public DateTime DateCreated { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? DateUpdated { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DateDeleted { get; set; }
}
