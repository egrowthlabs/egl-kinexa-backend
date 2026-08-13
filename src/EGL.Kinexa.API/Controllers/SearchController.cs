using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.Search.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EGL.Kinexa.API.Controllers;

[Route("api/search")]
public class SearchController : BaseController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] SearchProductsQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }
}
