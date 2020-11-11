using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IFrequenciaAlunoRepositorio
    {
        Task ExcluirFrequenciaAluno(FrequenciaAlunoSgpDto frequenciaAluno);
        Task<IEnumerable<FrequenciaAlunoResposta>> ObterFrequenciaAluno(int anoLetivo, string codigoUe, string codigoTurma, string codigoAluno);
        Task<IEnumerable<FrequenciaAlunoSgpDto>> ObterListaParaExclusao(int desdeAnoLetivo);
        Task SalvarFrequenciaAlunosBatch(IEnumerable<FrequenciaAlunoSgpDto> frequenciaAlunosSgp);
    }
}
