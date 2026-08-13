using System;

namespace EGL.Kinexa.Application.Features.ContactMessages.DTOs;

public record ContactMessageDto(
    int Id,
    string Name,
    string Email,
    string Phone,
    string Subject,
    string Message,
    bool IsRead,
    DateTime DateCreated
);
