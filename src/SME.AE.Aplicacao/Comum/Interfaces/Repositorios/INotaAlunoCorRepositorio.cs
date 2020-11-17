using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface INotaAlunoCorRepositorio
    {
        Task<IEnumerable<NotaAlunoCor>> ObterAsync();
    }
}