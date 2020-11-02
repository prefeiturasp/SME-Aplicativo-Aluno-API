using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IObterNotasAlunoUseCase
    {
        Task<IEnumerable<NotaAlunoResposta>> Executar(int anoLetivo, string codigoUe, string codigoTurma, string codigoAluno);
    }
}

