using SME.AE.Aplicacao.Comum.Modelos;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public interface IAtualizarDadosUsuarioUseCase
    {
        Task<RespostaApi> Executar(AtualizarDadosUsuarioDto usuarioDto);
    }
}
