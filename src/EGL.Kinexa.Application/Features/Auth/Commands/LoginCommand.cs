using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.Auth.DTOs;
using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EGL.Kinexa.Application.Features.Auth.Commands;

public class LoginCommand : IRequest<ApiResponse<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    public async Task<ApiResponse<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return ApiResponse<AuthResponse>.Fail("Invalid credentials.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return ApiResponse<AuthResponse>.Fail("Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);
        var authResponse = _jwtService.GenerateToken(user, roles);

        return ApiResponse<AuthResponse>.Ok(authResponse);
    }
}
