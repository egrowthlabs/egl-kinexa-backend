using System;
using System.Collections.Generic;

namespace EGL.Kinexa.Application.Features.Auth.DTOs;

public record AuthResponse(
    string Token,
    DateTime Expiration,
    UserInfo UserInfo
);

public record UserInfo(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    IList<string> Roles
);
