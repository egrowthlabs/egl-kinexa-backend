using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.Categories.Commands;
using EGL.Kinexa.Application.Features.Categories.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EGL.Kinexa.API.Controllers;

public class CategoriesController : BaseController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> List([FromQuery] GetCategoriesQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,CreadorContenido")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador,CreadorContenido")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryCommand command)
    {
        if (id != command.Id) return BadRequest(ApiResponse<object>.Fail("ID mismatch"));
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador,CreadorContenido")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await Mediator.Send(new DeleteCategoryCommand { Id = id });
        return HandleResult(result);
    }
}
