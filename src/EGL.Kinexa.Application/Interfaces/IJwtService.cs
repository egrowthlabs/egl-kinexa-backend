using System.Collections.Generic;
using EGL.Kinexa.Application.Features.Auth.DTOs;
using EGL.Kinexa.Domain.Entities;

namespace EGL.Kinexa.Application.Interfaces;

public interface IJwtService
{
    AuthResponse GenerateToken(ApplicationUser user, IList<string> roles);
}
