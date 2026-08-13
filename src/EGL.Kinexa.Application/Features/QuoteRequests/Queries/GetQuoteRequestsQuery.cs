using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.QuoteRequests.DTOs;
using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EGL.Kinexa.Application.Features.QuoteRequests.Queries;

public class GetQuoteRequestsQuery : IRequest<ApiResponse<PagedResult<QuoteRequestListDto>>>
{
    public PaginationParams Pagination { get; set; } = new();
    public QuoteRequestStatus? Status { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetQuoteRequestsQueryHandler : IRequestHandler<GetQuoteRequestsQuery, ApiResponse<PagedResult<QuoteRequestListDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetQuoteRequestsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<QuoteRequestListDto>>> Handle(GetQuoteRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.QuoteRequests.Queryable
            .Include(q => q.QuoteItems)
            .Where(q => !q.IsDeleted);

        if (request.Status.HasValue)
            query = query.Where(q => q.Status == request.Status.Value);

        if (!string.IsNullOrEmpty(request.SearchTerm))
            query = query.Where(q => q.CustomerName.Contains(request.SearchTerm) || q.CustomerEmail.Contains(request.SearchTerm));

        if (request.FromDate.HasValue)
            query = query.Where(q => q.DateCreated >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(q => q.DateCreated <= request.ToDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(q => q.DateCreated)
            .Skip((request.Pagination.PageNumber - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .Select(q => new QuoteRequestListDto(
                q.Id, q.CustomerName, q.CustomerEmail,
                (int)q.Status, q.Status.ToString(),
                q.QuoteItems.Count, q.DateCreated
            ))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<QuoteRequestListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.Pagination.PageNumber,
            PageSize = request.Pagination.PageSize
        };

        return ApiResponse<PagedResult<QuoteRequestListDto>>.Ok(result);
    }
}
