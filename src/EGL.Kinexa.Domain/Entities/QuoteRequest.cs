using System.Collections.Generic;
using EGL.Kinexa.Domain.Common;
using EGL.Kinexa.Domain.Enums;

namespace EGL.Kinexa.Domain.Entities;

public class QuoteRequest : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public QuoteRequestStatus Status { get; set; } = QuoteRequestStatus.Pendiente;
    public string? Notes { get; set; }

    public ICollection<QuoteItem> QuoteItems { get; set; } = new List<QuoteItem>();
}
