using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.Categories.DTOs;
using EGL.Kinexa.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EGL.Kinexa.Application.Features.Categories.Queries;

public class GetCategoriesQuery : IRequest<ApiResponse<List<CategoryDto>>>
{
    public bool? ActiveOnly { get; set; }
}

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, ApiResponse<List<CategoryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCategoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Categories.Queryable
            .Include(c => c.Products)
            .Where(c => !c.IsDeleted);

        if (request.ActiveOnly == true)
            query = query.Where(c => c.IsActive);

        var categories = await query
            .OrderBy(c => c.SortOrder)
            .Select(c => new CategoryDto(
                c.Id, c.Name, c.Slug, c.Description, c.LogoUrl, c.IsActive, c.SortOrder,
                c.Products.Count(p => !p.IsDeleted)
            ))
            .ToListAsync(cancellationToken);

        return ApiResponse<List<CategoryDto>>.Ok(categories);
    }
}
