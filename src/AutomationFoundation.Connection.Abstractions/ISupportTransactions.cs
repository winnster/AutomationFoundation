using System.Threading;
using System.Threading.Tasks;

namespace AutomationFoundation.Connection.Abstractions
{
    public interface ISupportTransactions
    {
        Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    }
}