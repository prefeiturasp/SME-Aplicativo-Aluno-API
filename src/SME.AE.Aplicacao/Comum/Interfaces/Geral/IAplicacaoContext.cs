using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Geral
{
    public interface IAplicacaoContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}