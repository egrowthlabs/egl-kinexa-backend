using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.MedicalBranches.DTOs;
using EGL.Kinexa.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EGL.Kinexa.Application.Features.MedicalBranches.Queries;

public class GetMedicalBranchesQuery : IRequest<ApiResponse<List<MedicalBranchDto>>>
{
    public bool? ActiveOnly { get; set; }
}

public class GetMedicalBranchesQueryHandler : IRequestHandler<GetMedicalBranchesQuery, ApiResponse<List<MedicalBranchDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMedicalBranchesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<MedicalBranchDto>>> Handle(GetMedicalBranchesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.MedicalBranches.Queryable
            .Include(m => m.Products)
            .Where(m => !m.IsDeleted);

        if (request.ActiveOnly == true)
            query = query.Where(m => m.IsActive);

        var branches = await query
            .OrderBy(m => m.SortOrder)
            .Select(m => new MedicalBranchDto(
                m.Id, m.Name, m.Slug, m.Description, m.IconUrl, m.IsActive, m.SortOrder,
                m.Products.Count(p => !p.IsDeleted)
            ))
            .ToListAsync(cancellationToken);

        return ApiResponse<List<MedicalBranchDto>>.Ok(branches);
    }
}
