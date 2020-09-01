using MediatR;
using SME.AE.Aplicacao.Comandos.Usuario.ReiniciarSenha;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using SME.AE.Comum.Excecoes;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class SolicitarReiniciarSenhaUseCase : ISolicitarReiniciarSenhaUseCase
    {
        private readonly IMediator mediator;

        public SolicitarReiniciarSenhaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RespostaApi> Executar(GerarTokenDto gerarTokenDto)
        {

            // Valida eh CPF Válido
            // Mensagem CPF inválido

            // Aluno eh ativo e tem responsável associado
            // Mensagem padrão: Este CPF não está vinculado...

            // A senha do usuário 999.999.999-99 - Nome do usuário foi reiniciada com sucesso. No próximo acesso ao aplicativo o usuário deverá informar a data de nascimento de um dos estudantes que é responsável.

            // Se o usuário nunca acessou o APP antes, então ele não existirá na base do Escola aqui e a mensagem que deverá ser apresentada para o usuário do SGP é: 
            // O usuário 999.999.999-99 - Nome do usuário deverá informar a data de nascimento de um dos estudantes que é responsável no campo de senha.


            var usuario = await mediator.Send(new ObterUsuarioQuery(gerarTokenDto.CPF));

            if (usuario == null)
                throw new NegocioException("CPF não encontrado");

            var usuarioCoreSSO = await mediator.Send(new ObterUsuarioCoreSSOQuery(gerarTokenDto.CPF));

            if (usuarioCoreSSO == null)
                throw new NegocioException("CPF não encontrado");

            await mediator.Send(new ReiniciarSenhaCommand() { Id = usuario.Id, PrimeiroAcesso = true });

            return RespostaApi.Sucesso();
        }

    }
}
