using System;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.MedicalBranches.DTOs;
using EGL.Kinexa.Application.Helpers;
using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Domain.Entities;
using MediatR;

namespace EGL.Kinexa.Application.Features.MedicalBranches.Commands;

public class CreateMedicalBranchCommand : IRequest<ApiResponse<MedicalBranchDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class CreateMedicalBranchCommandHandler : IRequestHandler<CreateMedicalBranchCommand, ApiResponse<MedicalBranchDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateMedicalBranchCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<MedicalBranchDto>> Handle(CreateMedicalBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = new MedicalBranch
        {
            Name = request.Name,
            Slug = SlugHelper.GenerateSlug(request.Name),
            Description = request.Description,
            IconUrl = request.IconUrl,
            IsActive = request.IsActive,
            SortOrder = request.SortOrder,
            CreatedBy = _currentUserService.UserId ?? "system",
            DateCreated = DateTime.UtcNow
        };

        await _unitOfWork.MedicalBranches.AddAsync(branch);
        await _unitOfWork.SaveAsync(cancellationToken);

        var dto = new MedicalBranchDto(
            branch.Id, branch.Name, branch.Slug, branch.Description,
            branch.IconUrl, branch.IsActive, branch.SortOrder, 0
        );

        return ApiResponse<MedicalBranchDto>.Ok(dto);
    }
}
