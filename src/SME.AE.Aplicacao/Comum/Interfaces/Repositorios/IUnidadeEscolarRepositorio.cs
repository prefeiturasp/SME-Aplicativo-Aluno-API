using SME.AE.Aplicacao.Comum.Modelos.Resposta.UnidadeEscolar;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IUnidadeEscolarRepositorio
    {
        Task<UnidadeEscolarResposta> ObterDadosUnidadeEscolarPorCodigoUe(string codigoUe);
    }
}