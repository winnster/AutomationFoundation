using System.Threading;
using System.Threading.Tasks;

namespace AutomationFoundation.Connection.Abstractions
{
    public interface IConnection
    {
        Task OpenAsync(CancellationToken cancellationToken);

        Task CloseAsync(CancellationToken cancellationToken);
    }
}