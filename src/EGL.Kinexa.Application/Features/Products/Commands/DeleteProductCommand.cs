using System;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Interfaces;
using MediatR;

namespace EGL.Kinexa.Application.Features.Products.Commands;

public class DeleteProductCommand : IRequest<ApiResponse<bool>>
{
    public int Id { get; set; }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
        if (product == null || product.IsDeleted)
            return ApiResponse<bool>.Fail("Product not found.");

        product.IsDeleted = true;
        product.DateDeleted = DateTime.UtcNow;
        product.DeletedBy = _currentUserService.UserId;
        product.Slug = $"deleted-{product.Id}-{product.Slug}"; // Free slug for reuse

        _unitOfWork.Products.SoftDelete(product);
        await _unitOfWork.SaveAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}
