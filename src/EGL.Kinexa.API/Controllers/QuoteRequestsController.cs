using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.QuoteRequests.Commands;
using EGL.Kinexa.Application.Features.QuoteRequests.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EGL.Kinexa.API.Controllers;

[Route("api/quote-requests")]
public class QuoteRequestsController : BaseController
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateQuoteRequestCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,VisorCotizaciones")]
    public async Task<IActionResult> List([FromQuery] GetQuoteRequestsQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrador,VisorCotizaciones")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await Mediator.Send(new GetQuoteRequestByIdQuery { Id = id });
        return HandleResult(result);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Administrador,VisorCotizaciones")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateQuoteStatusCommand command)
    {
        if (id != command.Id) return BadRequest(ApiResponse<object>.Fail("ID mismatch"));
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
