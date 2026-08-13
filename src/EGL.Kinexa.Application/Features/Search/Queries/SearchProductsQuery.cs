using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.Products.DTOs;
using EGL.Kinexa.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EGL.Kinexa.Application.Features.Search.Queries;

public class SearchProductsQuery : IRequest<ApiResponse<PagedResult<ProductListDto>>>
{
    public string SearchTerm { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public int? MedicalBranchId { get; set; }
    public PaginationParams Pagination { get; set; } = new();
}

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, ApiResponse<PagedResult<ProductListDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<ProductListDto>>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Products.Queryable
            .Include(p => p.Category)
            .Include(p => p.MedicalBranch)
            .Where(p => !p.IsDeleted && p.IsActive);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(p => 
                p.Name.ToLower().Contains(term) || 
                p.Description.ToLower().Contains(term) || 
                (p.SeoKeywords != null && p.SeoKeywords.ToLower().Contains(term)));
        }

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);

        if (request.MedicalBranchId.HasValue)
            query = query.Where(p => p.MedicalBranchId == request.MedicalBranchId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.SortOrder)
            .Skip((request.Pagination.PageNumber - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .Select(p => new ProductListDto(
                p.Id, p.Name, p.Slug, p.Category.Name, p.MedicalBranch.Name, p.ImageUrl, p.IsActive
            ))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<ProductListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.Pagination.PageNumber,
            PageSize = request.Pagination.PageSize
        };

        return ApiResponse<PagedResult<ProductListDto>>.Ok(result);
    }
}
