using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface INotaAlunoRepositorio
    {
        Task ExcluirNotaAluno(NotaAlunoSgpDto notaAluno);

        Task<IEnumerable<NotaAlunoSgpDto>> ObterListaParaExclusao(int desdeAnoLetivo, string ueCodigo);

        Task SalvarNotaAlunosBatch(IEnumerable<NotaAlunoSgpDto> notaAlunosSgp);

        Task<NotaAlunoPorBimestreResposta> ObterNotasAluno(int anoLetivo, short bimestre, string codigoUe, string codigoTurma, string codigoAluno);
    }
}