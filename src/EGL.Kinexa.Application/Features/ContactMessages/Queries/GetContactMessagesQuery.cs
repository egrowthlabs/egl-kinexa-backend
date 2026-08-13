using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.ContactMessages.DTOs;
using EGL.Kinexa.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EGL.Kinexa.Application.Features.ContactMessages.Queries;

public class GetContactMessagesQuery : IRequest<ApiResponse<PagedResult<ContactMessageDto>>>
{
    public PaginationParams Pagination { get; set; } = new();
    public bool? IsRead { get; set; }
}

public class GetContactMessagesQueryHandler : IRequestHandler<GetContactMessagesQuery, ApiResponse<PagedResult<ContactMessageDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetContactMessagesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<ContactMessageDto>>> Handle(GetContactMessagesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.ContactMessages.Queryable.Where(c => !c.IsDeleted);

        if (request.IsRead.HasValue)
            query = query.Where(c => c.IsRead == request.IsRead.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.DateCreated)
            .Skip((request.Pagination.PageNumber - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .Select(c => new ContactMessageDto(
                c.Id, c.Name, c.Email, c.Phone, c.Subject, c.Message, c.IsRead, c.DateCreated
            ))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<ContactMessageDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.Pagination.PageNumber,
            PageSize = request.Pagination.PageSize
        };

        return ApiResponse<PagedResult<ContactMessageDto>>.Ok(result);
    }
}
