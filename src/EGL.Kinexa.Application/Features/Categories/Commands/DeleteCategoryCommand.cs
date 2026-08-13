using System;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Interfaces;
using MediatR;

namespace EGL.Kinexa.Application.Features.Categories.Commands;

public class DeleteCategoryCommand : IRequest<ApiResponse<bool>>
{
    public int Id { get; set; }
}

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id);
        if (category == null || category.IsDeleted)
            return ApiResponse<bool>.Fail("Category not found.");

        category.IsDeleted = true;
        category.DateDeleted = DateTime.UtcNow;
        category.DeletedBy = _currentUserService.UserId;

        _unitOfWork.Categories.SoftDelete(category);
        await _unitOfWork.SaveAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}
