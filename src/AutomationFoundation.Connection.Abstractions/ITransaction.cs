using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationFoundation.Connection.Abstractions
{
    public interface ITransaction
    {
        Task CommitAsync(CancellationToken cancellationToken);

        Task RollbackAsync(CancellationToken cancellationToken);
    }
}