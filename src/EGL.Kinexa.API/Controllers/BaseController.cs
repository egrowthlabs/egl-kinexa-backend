using EGL.Kinexa.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EGL.Kinexa.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected ActionResult HandleResult<T>(ApiResponse<T> result)
    {
        if (result == null) return NotFound();
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }
}
