using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard
{
    public interface IObterTotaisAdesaoAgrupadosPorDreUseCase
    {
        Task<IEnumerable<TotaisAdesaoResultado>> Executar();
    }
}
