using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface INotaAlunoSgpRepositorio
    {
        Task<IEnumerable<NotaAlunoSgpDto>> ObterNotaAlunoSgp(int desdeAnoLetivo);
    }
}
