using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using EGL.Kinexa.Application.Common;
using EGL.Kinexa.Application.Helpers;
using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EGL.Kinexa.Application.Features.Products.Commands;

public class ImportProductsCommand : IRequest<ApiResponse<ImportProductsResult>>
{
    public IFormFile File { get; set; } = null!;
}

public class ImportProductsResult
{
    public int TotalRows { get; set; }
    public int Imported { get; set; }
    public int Skipped { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class ImportProductsCommandHandler : IRequestHandler<ImportProductsCommand, ApiResponse<ImportProductsResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ImportProductsCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<ImportProductsResult>> Handle(ImportProductsCommand request, CancellationToken cancellationToken)
    {
        var result = new ImportProductsResult();
        
        if (request.File == null || request.File.Length == 0)
            return ApiResponse<ImportProductsResult>.Fail("No se proporcionó un archivo válido.");

        var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        if (extension != ".xlsx" && extension != ".xls")
            return ApiResponse<ImportProductsResult>.Fail("Solo se permiten archivos Excel (.xlsx, .xls).");

        // Load categories and branches for name-to-ID mapping
        var categories = (await _unitOfWork.Categories.GetAllAsync()).ToList();
        var branches = (await _unitOfWork.MedicalBranches.GetAllAsync()).ToList();

        using var stream = new MemoryStream();
        await request.File.CopyToAsync(stream, cancellationToken);
        stream.Position = 0;

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RangeUsed()?.RowsUsed().Skip(1); // Skip header

        if (rows == null)
            return ApiResponse<ImportProductsResult>.Fail("El archivo está vacío.");

        var products = new List<Product>();
        var rowNumber = 1;

        foreach (var row in rows)
        {
            rowNumber++;
            try
            {
                var name = row.Cell(1).GetString().Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    result.Errors.Add($"Fila {rowNumber}: Nombre vacío, se omitió.");
                    result.Skipped++;
                    continue;
                }

                var description = row.Cell(2).GetString().Trim();
                var categoryName = row.Cell(3).GetString().Trim();
                var branchName = row.Cell(4).GetString().Trim();

                // Resolve category
                var category = categories.FirstOrDefault(c =>
                    c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
                if (category == null)
                {
                    result.Errors.Add($"Fila {rowNumber}: Categoría '{categoryName}' no encontrada.");
                    result.Skipped++;
                    continue;
                }

                // Resolve branch
                var branch = branches.FirstOrDefault(b =>
                    b.Name.Equals(branchName, StringComparison.OrdinalIgnoreCase));
                if (branch == null)
                {
                    result.Errors.Add($"Fila {rowNumber}: Rama médica '{branchName}' no encontrada.");
                    result.Skipped++;
                    continue;
                }

                var product = new Product
                {
                    Name = name,
                    Slug = SlugHelper.GenerateSlug(name),
                    Description = description,
                    CategoryId = category.Id,
                    MedicalBranchId = branch.Id,
                    Material = row.Cell(5).GetString().Trim(),
                    MaterialType = row.Cell(6).GetString().Trim(),
                    Measurements = row.Cell(7).GetString().Trim(),
                    UsageIndications = row.Cell(8).GetString().Trim(),
                    SpecificInstruments = row.Cell(9).GetString().Trim(),
                    Competitors = row.Cell(10).GetString().Trim(),
                    SeoKeywords = row.Cell(11).GetString().Trim(),
                    ImageUrl = row.Cell(12).GetString().Trim(),
                    IsActive = true,
                    SortOrder = 0,
                    CreatedBy = _currentUserService.UserId ?? "system",
                    DateCreated = DateTime.UtcNow
                };

                products.Add(product);
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Fila {rowNumber}: Error - {ex.Message}");
                result.Skipped++;
            }
        }

        if (products.Any())
        {
            foreach (var product in products)
            {
                await _unitOfWork.Products.AddAsync(product);
            }
            await _unitOfWork.SaveAsync(cancellationToken);
        }

        result.TotalRows = rowNumber - 1;
        result.Imported = products.Count;
        result.Skipped = result.TotalRows - result.Imported;

        return ApiResponse<ImportProductsResult>.Ok(result);
    }
}
