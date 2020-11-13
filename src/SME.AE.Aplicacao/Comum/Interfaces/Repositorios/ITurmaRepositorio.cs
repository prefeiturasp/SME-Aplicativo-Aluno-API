using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface ITurmaRepositorio
    {
        Task<TurmaModalidadeDeEnsinoDto> ObterModalidadeDeEnsino(string codigoTurma);
    }
}