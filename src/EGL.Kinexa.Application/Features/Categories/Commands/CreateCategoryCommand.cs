using System;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.Categories.DTOs;
using EGL.Kinexa.Application.Helpers;
using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Domain.Entities;
using MediatR;

namespace EGL.Kinexa.Application.Features.Categories.Commands;

public class CreateCategoryCommand : IRequest<ApiResponse<CategoryDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ApiResponse<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name,
            Slug = SlugHelper.GenerateSlug(request.Name),
            Description = request.Description,
            LogoUrl = request.LogoUrl,
            IsActive = request.IsActive,
            SortOrder = request.SortOrder,
            CreatedBy = _currentUserService.UserId ?? "system",
            DateCreated = DateTime.UtcNow
        };

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveAsync(cancellationToken);

        var dto = new CategoryDto(
            category.Id, category.Name, category.Slug, category.Description,
            category.LogoUrl, category.IsActive, category.SortOrder, 0
        );

        return ApiResponse<CategoryDto>.Ok(dto);
    }
}
