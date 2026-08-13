using System;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Interfaces;
using MediatR;

namespace EGL.Kinexa.Application.Features.ContactMessages.Commands;

public class MarkAsReadCommand : IRequest<ApiResponse<bool>>
{
    public int Id { get; set; }
}

public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public MarkAsReadCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<bool>> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        var message = await _unitOfWork.ContactMessages.GetByIdAsync(request.Id);
        if (message == null || message.IsDeleted)
            return ApiResponse<bool>.Fail("Message not found.");

        message.IsRead = true;
        message.UpdatedBy = _currentUserService.UserId;
        message.DateUpdated = DateTime.UtcNow;

        _unitOfWork.ContactMessages.Update(message);
        await _unitOfWork.SaveAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}
