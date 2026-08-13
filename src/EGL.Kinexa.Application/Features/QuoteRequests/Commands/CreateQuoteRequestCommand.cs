using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.QuoteRequests.DTOs;
using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Domain.Entities;
using EGL.Kinexa.Domain.Enums;
using MediatR;

namespace EGL.Kinexa.Application.Features.QuoteRequests.Commands;

public class CreateQuoteItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}

public class CreateQuoteRequestCommand : IRequest<ApiResponse<QuoteRequestDto>>
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<CreateQuoteItemDto> Items { get; set; } = new();
}

public class CreateQuoteRequestCommandHandler : IRequestHandler<CreateQuoteRequestCommand, ApiResponse<QuoteRequestDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public CreateQuoteRequestCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<ApiResponse<QuoteRequestDto>> Handle(CreateQuoteRequestCommand request, CancellationToken cancellationToken)
    {
        var quoteRequest = new QuoteRequest
        {
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            CustomerEmail = request.CustomerEmail,
            Notes = request.Notes,
            Status = QuoteRequestStatus.Pendiente,
            DateCreated = DateTime.UtcNow,
            CreatedBy = "system"
        };

        foreach (var item in request.Items)
        {
            quoteRequest.QuoteItems.Add(new QuoteItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Notes = item.Notes,
                DateCreated = DateTime.UtcNow,
                CreatedBy = "system"
            });
        }

        await _unitOfWork.QuoteRequests.AddAsync(quoteRequest);
        await _unitOfWork.SaveAsync(cancellationToken);

        // Send notifications
        await _emailService.SendEmailAsync("comercial@kinexa.com.mx", "New Quote Request", 
            $"A new quote request has been submitted by {request.CustomerName}.");
            
        await _emailService.SendEmailAsync(request.CustomerEmail, "Quote Request Received", 
            "We have received your quote request and will get back to you soon.");

        // For simplicity, we just return the basic info in DTO. Ideally, requery to include product names.
        var dto = new QuoteRequestDto(
            quoteRequest.Id, quoteRequest.CustomerName, quoteRequest.CustomerPhone,
            quoteRequest.CustomerEmail, (int)quoteRequest.Status, quoteRequest.Status.ToString(),
            quoteRequest.Notes, quoteRequest.DateCreated, new List<QuoteItemDto>()
        );

        return ApiResponse<QuoteRequestDto>.Ok(dto);
    }
}
