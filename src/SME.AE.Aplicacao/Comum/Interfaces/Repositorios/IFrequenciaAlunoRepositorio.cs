using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IFrequenciaAlunoRepositorio
    {
        Task<IEnumerable<FrequenciaAlunoResposta>> ObterFrequenciaAluno(int anoLetivo, string codigoUe, long codigoTurma, string codigoAluno);
    }
}
