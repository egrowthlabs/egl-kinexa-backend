using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.Products.DTOs;
using EGL.Kinexa.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EGL.Kinexa.Application.Features.Products.Queries;

public class GetProductsQuery : IRequest<ApiResponse<PagedResult<ProductListDto>>>
{
    public PaginationParams Pagination { get; set; } = new();
    public int? CategoryId { get; set; }
    public int? MedicalBranchId { get; set; }
    public string? SearchTerm { get; set; }
}

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, ApiResponse<PagedResult<ProductListDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<ProductListDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Products.Queryable
            .Include(p => p.Category)
            .Include(p => p.MedicalBranch)
            .Where(p => !p.IsDeleted && p.IsActive);

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
            
        if (request.MedicalBranchId.HasValue)
            query = query.Where(p => p.MedicalBranchId == request.MedicalBranchId.Value);

        if (!string.IsNullOrEmpty(request.SearchTerm))
            query = query.Where(p => p.Name.Contains(request.SearchTerm) || p.Description.Contains(request.SearchTerm));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.SortOrder)
            .Skip((request.Pagination.PageNumber - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .Select(p => new ProductListDto(
                p.Id, p.Name, p.Slug, p.Category.Name, p.MedicalBranch.Name, p.ImageUrl, p.IsActive))
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
