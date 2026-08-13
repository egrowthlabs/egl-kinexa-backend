using EGL.Kinexa.Domain.Common;

namespace EGL.Kinexa.Domain.Entities;

public class QuoteItem : BaseEntity
{
    public int QuoteRequestId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }

    public QuoteRequest QuoteRequest { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
