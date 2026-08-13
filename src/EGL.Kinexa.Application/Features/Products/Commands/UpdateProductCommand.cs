using System;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Helpers;
using EGL.Kinexa.Application.Interfaces;
using MediatR;

namespace EGL.Kinexa.Application.Features.Products.Commands;

public class UpdateProductCommand : IRequest<ApiResponse<bool>>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int MedicalBranchId { get; set; }
    public string? SeoKeywords { get; set; }
    public string? UsageIndications { get; set; }
    public string? Material { get; set; }
    public string? MaterialType { get; set; }
    public string? Measurements { get; set; }
    public string? SpecificInstruments { get; set; }
    public string? Competitors { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<bool>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
        if (product == null || product.IsDeleted)
            return ApiResponse<bool>.Fail("Product not found.");

        if (product.Name != request.Name)
        {
            product.Name = request.Name;
            product.Slug = SlugHelper.GenerateSlug(request.Name);
        }

        product.Description = request.Description;
        product.CategoryId = request.CategoryId;
        product.MedicalBranchId = request.MedicalBranchId;
        product.SeoKeywords = request.SeoKeywords;
        product.UsageIndications = request.UsageIndications;
        product.Material = request.Material;
        product.MaterialType = request.MaterialType;
        product.Measurements = request.Measurements;
        product.SpecificInstruments = request.SpecificInstruments;
        product.Competitors = request.Competitors;
        product.ImageUrl = request.ImageUrl;
        product.IsActive = request.IsActive;
        product.SortOrder = request.SortOrder;

        product.UpdatedBy = _currentUserService.UserId;
        product.DateUpdated = DateTime.UtcNow;

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}
