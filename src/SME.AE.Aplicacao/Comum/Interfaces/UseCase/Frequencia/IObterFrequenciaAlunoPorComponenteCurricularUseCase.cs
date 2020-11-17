using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno.PorComponenteCurricular;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IObterFrequenciaAlunoPorComponenteCurricularUseCase
    {
        Task<FrequenciaAlunoPorComponenteCurricularResposta> Executar(ObterFrequenciaAlunoPorComponenteCurricularDto frequenciaAlunoDto);
    }
}