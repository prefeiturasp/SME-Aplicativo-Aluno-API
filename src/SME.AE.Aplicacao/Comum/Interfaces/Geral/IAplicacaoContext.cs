using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Geral
{
    public interface IAplicacaoContext
    {
        // Exemplo de declaracoa de DataSet para o contexto
        // DbSet<TodoItem> TodoItems { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}