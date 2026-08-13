using System;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Domain.Entities;
using MediatR;

namespace EGL.Kinexa.Application.Features.ContactMessages.Commands;

public class CreateContactMessageCommand : IRequest<ApiResponse<bool>>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class CreateContactMessageCommandHandler : IRequestHandler<CreateContactMessageCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public CreateContactMessageCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<ApiResponse<bool>> Handle(CreateContactMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new ContactMessage
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Subject = request.Subject,
            Message = request.Message,
            IsRead = false,
            CreatedBy = "system",
            DateCreated = DateTime.UtcNow
        };

        await _unitOfWork.ContactMessages.AddAsync(message);
        await _unitOfWork.SaveAsync(cancellationToken);

        await _emailService.SendEmailAsync("comercial@kinexa.com.mx", 
            $"New Contact Message: {request.Subject}", 
            $"You have received a new contact message from {request.Name}.");

        return ApiResponse<bool>.Ok(true);
    }
}
