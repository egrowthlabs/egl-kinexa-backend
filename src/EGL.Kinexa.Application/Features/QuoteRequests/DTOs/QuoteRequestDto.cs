using System;
using System.Collections.Generic;

namespace EGL.Kinexa.Application.Features.QuoteRequests.DTOs;

public record QuoteRequestDto(
    int Id,
    string CustomerName,
    string CustomerPhone,
    string CustomerEmail,
    int Status,
    string StatusName,
    string? Notes,
    DateTime DateCreated,
    List<QuoteItemDto> Items
);

public record QuoteItemDto(
    int Id,
    int ProductId,
    string ProductName,
    string ProductSlug,
    string? ProductImageUrl,
    int Quantity,
    string? Notes
);

public record QuoteRequestListDto(
    int Id,
    string CustomerName,
    string CustomerEmail,
    int Status,
    string StatusName,
    int ItemCount,
    DateTime DateCreated
);
