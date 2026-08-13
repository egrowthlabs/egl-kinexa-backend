using System;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Interfaces;
using MediatR;

namespace EGL.Kinexa.Application.Features.MedicalBranches.Commands;

public class DeleteMedicalBranchCommand : IRequest<ApiResponse<bool>>
{
    public int Id { get; set; }
}

public class DeleteMedicalBranchCommandHandler : IRequestHandler<DeleteMedicalBranchCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteMedicalBranchCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteMedicalBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _unitOfWork.MedicalBranches.GetByIdAsync(request.Id);
        if (branch == null || branch.IsDeleted)
            return ApiResponse<bool>.Fail("Medical branch not found.");

        branch.IsDeleted = true;
        branch.DateDeleted = DateTime.UtcNow;
        branch.DeletedBy = _currentUserService.UserId;

        _unitOfWork.MedicalBranches.SoftDelete(branch);
        await _unitOfWork.SaveAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}
