using MediatR;
using SME.AE.Aplicacao.CasoDeUso.Usuario.Excecoes;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterUsuarioUseCase : IObterUsuarioUseCase
    {
        private readonly IMediator mediator;

        public ObterUsuarioUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<UsuarioDto> Executar( string cpf)
        {
            var usuario = await mediator.Send(new ObterUsuarioQuery(cpf));
            if (usuario == null)
                throw new UsuarioNaoEncontradoException();
            return new UsuarioDto(usuario.Cpf, usuario.Nome);
        }
    }
}
