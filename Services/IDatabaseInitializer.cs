using System.Threading;
using System.Threading.Tasks;

namespace JapaneseTrainer.Api.Services
{
    public interface IDatabaseInitializer
    {
        Task EnsureCreatedAsync(CancellationToken cancellationToken = default);
    }
}


