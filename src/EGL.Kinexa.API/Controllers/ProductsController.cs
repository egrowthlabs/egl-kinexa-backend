using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Features.Products.Commands;
using EGL.Kinexa.Application.Features.Products.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System.IO;

namespace EGL.Kinexa.API.Controllers;

public class ProductsController : BaseController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> List([FromQuery] GetProductsQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await Mediator.Send(new GetProductByIdQuery { Id = id });
        return HandleResult(result);
    }

    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await Mediator.Send(new GetProductBySlugQuery { Slug = slug });
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,CreadorContenido")]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador,CreadorContenido")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id) return BadRequest(ApiResponse<object>.Fail("ID mismatch"));
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador,CreadorContenido")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await Mediator.Send(new DeleteProductCommand { Id = id });
        return HandleResult(result);
    }

    [HttpPost("import")]
    [Authorize(Roles = "Administrador,CreadorContenido")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Import([FromForm] ImportProductsCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("import-template")]
    [Authorize(Roles = "Administrador,CreadorContenido")]
    public IActionResult DownloadTemplate()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Productos");
        
        var headers = new[] { "Nombre*", "Descripción*", "Categoría*", "Rama Médica*", "Material", "Tipo Material", "Medidas", "Indicaciones de Uso", "Instrumentos Específicos", "Competidores", "SEO Keywords", "URL Imagen" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#0B2D59");
            ws.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
        }
        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "plantilla_productos_kinexa.xlsx");
    }

    /// <summary>Admin: Fix slugs of soft-deleted products so they can be reused</summary>
    [HttpPost("fix-deleted-slugs")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> FixDeletedSlugs(
        [FromServices] EGL.Kinexa.Persistence.Context.KinexaDbContext db,
        CancellationToken ct)
    {
        // Raw SQL to bypass global query filter and fix all soft-deleted slugs
        var affected = await db.Database.ExecuteSqlRawAsync(
            "UPDATE \"Products\" SET \"Slug\" = CONCAT('deleted-', \"Id\", '-', \"Slug\") WHERE \"IsDeleted\" = true AND \"Slug\" NOT LIKE 'deleted-%'",
            ct);
        return Ok(new { fixed_count = affected, message = $"Fixed {affected} soft-deleted product slugs." });
    }
}
