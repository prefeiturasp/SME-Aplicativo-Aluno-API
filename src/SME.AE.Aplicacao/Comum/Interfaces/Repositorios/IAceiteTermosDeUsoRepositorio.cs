using SME.AE.Dominio.Entidades;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IAceiteTermosDeUsoRepositorio : IBaseRepositorio<AceiteTermosDeUso>
    {
        Task<bool> ValidarAceiteDoTermoDeUsoPorUsuarioEVersao(string cpfUsuario, double versao);

        Task<bool> RegistrarAceite(AceiteTermosDeUso aceiteTermosDeUso);
    }
}