using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.Products.DTOs;
using EGL.Kinexa.Application.Helpers;
using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Domain.Entities;
using MediatR;

namespace EGL.Kinexa.Application.Features.Products.Commands;

public class CreateProductCommand : IRequest<ApiResponse<ProductDto>>
{
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

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ApiResponse<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Generate unique slug (handle conflicts with counter suffix)
        var baseSlug = SlugHelper.GenerateSlug(request.Name);
        var slug = baseSlug;
        var counter = 2;
        var existingSlugs = await _unitOfWork.Products.GetAllAsync();
        var takenSlugs = new HashSet<string>(existingSlugs.Where(p => !p.IsDeleted).Select(p => p.Slug));
        while (takenSlugs.Contains(slug))
        {
            slug = $"{baseSlug}-{counter++}";
        }

        var product = new Product
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            CategoryId = request.CategoryId,
            MedicalBranchId = request.MedicalBranchId,
            SeoKeywords = request.SeoKeywords,
            UsageIndications = request.UsageIndications,
            Material = request.Material,
            MaterialType = request.MaterialType,
            Measurements = request.Measurements,
            SpecificInstruments = request.SpecificInstruments,
            Competitors = request.Competitors,
            ImageUrl = request.ImageUrl,
            IsActive = request.IsActive,
            SortOrder = request.SortOrder,
            CreatedBy = _currentUserService.UserId ?? "system",
            DateCreated = DateTime.UtcNow
        };

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveAsync(cancellationToken);

        var dto = new ProductDto(
            product.Id, product.Name, product.Slug, product.Description,
            product.CategoryId, string.Empty, // Populate via query if needed
            product.MedicalBranchId, string.Empty,
            product.ImageUrl, product.IsActive, product.SortOrder
        );

        return ApiResponse<ProductDto>.Ok(dto);
    }
}
