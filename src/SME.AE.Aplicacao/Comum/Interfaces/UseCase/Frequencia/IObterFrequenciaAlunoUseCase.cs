using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase.Frequencia
{
    public interface IObterFrequenciaAlunoUseCase
    {
        Task<FrequenciaAlunoResposta> Executar(ObterFrequenciaAlunoDto frequenciaAlunoDto);
    }
}