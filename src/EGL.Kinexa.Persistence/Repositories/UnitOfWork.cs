using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Domain.Entities;
using EGL.Kinexa.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace EGL.Kinexa.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly KinexaDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    private IGenericRepository<Product>? _products;
    private IGenericRepository<Category>? _categories;
    private IGenericRepository<MedicalBranch>? _medicalBranches;
    private IGenericRepository<QuoteRequest>? _quoteRequests;
    private IGenericRepository<QuoteItem>? _quoteItems;
    private IGenericRepository<ContactMessage>? _contactMessages;

    public UnitOfWork(KinexaDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IGenericRepository<Product> Products =>
        _products ??= new GenericRepository<Product>(_context);

    public IGenericRepository<Category> Categories =>
        _categories ??= new GenericRepository<Category>(_context);

    public IGenericRepository<MedicalBranch> MedicalBranches =>
        _medicalBranches ??= new GenericRepository<MedicalBranch>(_context);

    public IGenericRepository<QuoteRequest> QuoteRequests =>
        _quoteRequests ??= new GenericRepository<QuoteRequest>(_context);

    public IGenericRepository<QuoteItem> QuoteItems =>
        _quoteItems ??= new GenericRepository<QuoteItem>(_context);

    public IGenericRepository<ContactMessage> ContactMessages =>
        _contactMessages ??= new GenericRepository<ContactMessage>(_context);

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null) return;
        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveAsync();
            if (_currentTransaction != null)
                await _currentTransaction.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_currentTransaction != null)
                await _currentTransaction.RollbackAsync();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
