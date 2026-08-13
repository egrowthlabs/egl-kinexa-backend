using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.ContactMessages.Commands;
using EGL.Kinexa.Application.Features.ContactMessages.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EGL.Kinexa.API.Controllers;

[Route("api/contact")]
public class ContactController : BaseController
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Submit([FromBody] CreateContactMessageCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> List([FromQuery] GetContactMessagesQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPut("{id:int}/read")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> MarkAsRead(int id, [FromBody] MarkAsReadCommand command)
    {
        if (id != command.Id) return BadRequest(ApiResponse<object>.Fail("ID mismatch"));
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
