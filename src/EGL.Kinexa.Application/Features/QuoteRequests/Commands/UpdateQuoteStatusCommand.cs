using System;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Domain.Enums;
using MediatR;

namespace EGL.Kinexa.Application.Features.QuoteRequests.Commands;

public class UpdateQuoteStatusCommand : IRequest<ApiResponse<bool>>
{
    public int Id { get; set; }
    public QuoteRequestStatus NewStatus { get; set; }
    public string? Notes { get; set; }
}

public class UpdateQuoteStatusCommandHandler : IRequestHandler<UpdateQuoteStatusCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateQuoteStatusCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<bool>> Handle(UpdateQuoteStatusCommand request, CancellationToken cancellationToken)
    {
        var quote = await _unitOfWork.QuoteRequests.GetByIdAsync(request.Id);
        if (quote == null || quote.IsDeleted)
            return ApiResponse<bool>.Fail("Quote request not found.");

        quote.Status = request.NewStatus;
        if (!string.IsNullOrEmpty(request.Notes))
        {
            quote.Notes = request.Notes;
        }

        quote.UpdatedBy = _currentUserService.UserId;
        quote.DateUpdated = DateTime.UtcNow;

        _unitOfWork.QuoteRequests.Update(quote);
        await _unitOfWork.SaveAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}
