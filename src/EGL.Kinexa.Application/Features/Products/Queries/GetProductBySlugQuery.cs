using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.Products.DTOs;
using EGL.Kinexa.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EGL.Kinexa.Application.Features.Products.Queries;

public class GetProductBySlugQuery : IRequest<ApiResponse<ProductDetailDto>>
{
    public string Slug { get; set; } = string.Empty;
}

public class GetProductBySlugQueryHandler : IRequestHandler<GetProductBySlugQuery, ApiResponse<ProductDetailDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductBySlugQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ProductDetailDto>> Handle(GetProductBySlugQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.Queryable
            .Include(p => p.Category)
            .Include(p => p.MedicalBranch)
            .FirstOrDefaultAsync(p => p.Slug == request.Slug && !p.IsDeleted, cancellationToken);

        if (product == null)
            return ApiResponse<ProductDetailDto>.Fail("Product not found.");

        var dto = new ProductDetailDto(
            product.Id, product.Name, product.Slug, product.Description,
            product.CategoryId, product.Category.Name,
            product.MedicalBranchId, product.MedicalBranch.Name,
            product.ImageUrl, product.IsActive, product.SortOrder,
            product.SeoKeywords, product.UsageIndications, product.Material,
            product.MaterialType, product.Measurements, product.SpecificInstruments,
            product.Competitors
        );

        return ApiResponse<ProductDetailDto>.Ok(dto);
    }
}
