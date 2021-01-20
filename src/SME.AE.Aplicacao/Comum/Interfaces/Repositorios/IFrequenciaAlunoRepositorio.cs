using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno.PorComponenteCurricular;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IFrequenciaAlunoRepositorio
    {
        Task ExcluirFrequenciaAluno(FrequenciaAlunoSgpDto frequenciaAluno);

        Task<FrequenciaAlunoPorComponenteCurricularResposta> ObterFrequenciaAlunoPorComponenteCurricularAsync(int anoLetivo, string codigoUe, string codigoTurma, string codigoAluno, short componenteCurricular);

        Task<FrequenciaAlunoResposta> ObterFrequenciaAlunoAsync(int anoLetivo, string codigoUe, string codigoTurma, string codigoAluno);

        Task<IEnumerable<FrequenciaAlunoSgpDto>> ObterListaParaExclusao(int desdeAnoLetivo);

        Task SalvarFrequenciaAlunosBatch(IEnumerable<FrequenciaAlunoSgpDto> frequenciaAlunosSgp);
    }
}