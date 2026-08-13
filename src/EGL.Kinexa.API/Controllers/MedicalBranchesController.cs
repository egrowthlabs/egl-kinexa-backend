using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.MedicalBranches.Commands;
using EGL.Kinexa.Application.Features.MedicalBranches.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EGL.Kinexa.API.Controllers;

[Route("api/medical-branches")]
public class MedicalBranchesController : BaseController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> List([FromQuery] GetMedicalBranchesQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,CreadorContenido")]
    public async Task<IActionResult> Create([FromBody] CreateMedicalBranchCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador,CreadorContenido")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicalBranchCommand command)
    {
        if (id != command.Id) return BadRequest(ApiResponse<object>.Fail("ID mismatch"));
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador,CreadorContenido")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await Mediator.Send(new DeleteMedicalBranchCommand { Id = id });
        return HandleResult(result);
    }
}
