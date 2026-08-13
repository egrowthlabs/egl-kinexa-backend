using System;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Helpers;
using EGL.Kinexa.Application.Interfaces;
using MediatR;

namespace EGL.Kinexa.Application.Features.MedicalBranches.Commands;

public class UpdateMedicalBranchCommand : IRequest<ApiResponse<bool>>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class UpdateMedicalBranchCommandHandler : IRequestHandler<UpdateMedicalBranchCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateMedicalBranchCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<bool>> Handle(UpdateMedicalBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _unitOfWork.MedicalBranches.GetByIdAsync(request.Id);
        if (branch == null || branch.IsDeleted)
            return ApiResponse<bool>.Fail("Medical branch not found.");

        if (branch.Name != request.Name)
        {
            branch.Name = request.Name;
            branch.Slug = SlugHelper.GenerateSlug(request.Name);
        }

        branch.Description = request.Description;
        branch.IconUrl = request.IconUrl;
        branch.IsActive = request.IsActive;
        branch.SortOrder = request.SortOrder;
        
        branch.UpdatedBy = _currentUserService.UserId;
        branch.DateUpdated = DateTime.UtcNow;

        _unitOfWork.MedicalBranches.Update(branch);
        await _unitOfWork.SaveAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}
