using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.CasoDeUso.Usuario.Excecoes;
using SME.AE.Aplicacao.Comandos.Usuario.ObterPorCpf;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class ObterUsuarioPorCpfUseCase
    {
        public static async Task<Dominio.Entidades.Usuario> Executar(IMediator mediator,  string cpf)
        {
            Dominio.Entidades.Usuario usuario = await mediator.Send(new ObterUsuarioPorCpfCommand(cpf));
            if (usuario == null)
                throw new UsuarioNaoEncontradoException();
            return usuario;
        }
    }
}