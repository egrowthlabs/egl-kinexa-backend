using System;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Helpers;
using EGL.Kinexa.Application.Interfaces;
using MediatR;

namespace EGL.Kinexa.Application.Features.Categories.Commands;

public class UpdateCategoryCommand : IRequest<ApiResponse<bool>>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id);
        if (category == null || category.IsDeleted)
            return ApiResponse<bool>.Fail("Category not found.");

        if (category.Name != request.Name)
        {
            category.Name = request.Name;
            category.Slug = SlugHelper.GenerateSlug(request.Name);
        }

        category.Description = request.Description;
        category.LogoUrl = request.LogoUrl;
        category.IsActive = request.IsActive;
        category.SortOrder = request.SortOrder;
        
        category.UpdatedBy = _currentUserService.UserId;
        category.DateUpdated = DateTime.UtcNow;

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}
