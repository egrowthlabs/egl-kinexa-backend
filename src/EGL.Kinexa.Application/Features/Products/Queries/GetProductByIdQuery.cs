using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.Products.DTOs;
using EGL.Kinexa.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EGL.Kinexa.Application.Features.Products.Queries;

public class GetProductByIdQuery : IRequest<ApiResponse<ProductDetailDto>>
{
    public int Id { get; set; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ApiResponse<ProductDetailDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ProductDetailDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.Queryable
            .Include(p => p.Category)
            .Include(p => p.MedicalBranch)
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

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
