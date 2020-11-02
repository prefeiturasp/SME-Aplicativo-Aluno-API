using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface INotaAlunoRepositorio
    {
        Task SalvarNotaAlunosBatch(IEnumerable<NotaAlunoSgpDto> notaAlunosSgp);

        Task<IEnumerable<NotaAlunoResposta>> ObterNotasAluno(int anoLetivo, string codigoUe, string codigoTurma, string codigoAluno);
    }
}
