using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IObterEventosAlunoPorMesUseCase
    {
        Task<IEnumerable<EventoRespostaDto>> Executar(string cpf, long codigoAluno, int mes, int ano);
    }
}
