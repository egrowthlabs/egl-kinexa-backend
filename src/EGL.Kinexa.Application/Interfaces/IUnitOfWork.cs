using System.Threading;
using System.Threading.Tasks;
using EGL.Kinexa.Domain.Entities;

namespace EGL.Kinexa.Application.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<Product> Products { get; }
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<MedicalBranch> MedicalBranches { get; }
    IGenericRepository<QuoteRequest> QuoteRequests { get; }
    IGenericRepository<QuoteItem> QuoteItems { get; }
    IGenericRepository<ContactMessage> ContactMessages { get; }

    Task<int> SaveAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
