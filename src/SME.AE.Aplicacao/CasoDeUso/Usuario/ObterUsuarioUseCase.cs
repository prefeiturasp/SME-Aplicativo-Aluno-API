using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Comum.Excecoes;
using SME.AE.Comum.Utilitarios;
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

        public async Task<UsuarioDto> Executar(string cpf)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorCpfQuery(cpf));
            if (usuario == null)
                throw new NegocioException($"O usuário {Formatacao.FormatarCpf(cpf)} deverá informar a data de nascimento de um dos estudantes que é responsável no campo de senha!");



            return new UsuarioDto(usuario.Cpf, usuario.Nome);
        }
    }
}