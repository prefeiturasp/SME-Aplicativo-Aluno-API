using SME.AE.Aplicacao.Comum.Modelos.Resposta.UnidadeEscolar;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IObterDadosUnidadeEscolarUseCase
    {
        Task<UnidadeEscolarResposta> Executar(string codigoUe);
    }
}