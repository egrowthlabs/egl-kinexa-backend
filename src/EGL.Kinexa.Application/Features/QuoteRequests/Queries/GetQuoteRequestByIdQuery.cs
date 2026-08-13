using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.QuoteRequests.DTOs;
using EGL.Kinexa.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EGL.Kinexa.Application.Features.QuoteRequests.Queries;

public class GetQuoteRequestByIdQuery : IRequest<ApiResponse<QuoteRequestDto>>
{
    public int Id { get; set; }
}

public class GetQuoteRequestByIdQueryHandler : IRequestHandler<GetQuoteRequestByIdQuery, ApiResponse<QuoteRequestDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetQuoteRequestByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<QuoteRequestDto>> Handle(GetQuoteRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var quote = await _unitOfWork.QuoteRequests.Queryable
            .Include(q => q.QuoteItems)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(q => q.Id == request.Id && !q.IsDeleted, cancellationToken);

        if (quote == null)
            return ApiResponse<QuoteRequestDto>.Fail("Quote request not found.");

        var dto = new QuoteRequestDto(
            quote.Id, quote.CustomerName, quote.CustomerPhone, quote.CustomerEmail,
            (int)quote.Status, quote.Status.ToString(), quote.Notes, quote.DateCreated,
            quote.QuoteItems.Select(i => new QuoteItemDto(
                i.Id, i.ProductId, i.Product.Name, i.Product.Slug, i.Product.ImageUrl, i.Quantity, i.Notes
            )).ToList()
        );

        return ApiResponse<QuoteRequestDto>.Ok(dto);
    }
}
