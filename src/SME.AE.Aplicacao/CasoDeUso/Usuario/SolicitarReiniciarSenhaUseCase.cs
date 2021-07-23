using MediatR;
using SME.AE.Aplicacao.Comandos.Usuario.ReiniciarSenha;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using SME.AE.Comum.Excecoes;
using SME.AE.Comum.Utilitarios;
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

        public async Task<RespostaApi> Executar(SolicitarReiniciarSenhaDto solicitarReiniciarSenhaDto)
        {

            if (ValidacaoCpf.Valida(solicitarReiniciarSenhaDto.Cpf) == false)
                throw new NegocioException($"CPF inválido!");

            var usuarioCoreSSO = await mediator.Send(new ObterUsuarioCoreSSOQuery(solicitarReiniciarSenhaDto.Cpf));

            await mediator.Send(new ObterDadosAlunosQuery(solicitarReiniciarSenhaDto.Cpf));

            var usuario = await mediator.Send(new ObterUsuarioPorCpfQuery(solicitarReiniciarSenhaDto.Cpf));

            if (usuario == null && usuarioCoreSSO != null)
                throw new NegocioException($"O usuário {Formatacao.FormatarCpf(solicitarReiniciarSenhaDto.Cpf)} deverá informar a data de nascimento de um dos estudantes que é responsável no campo de senha!");

            var usuarioEol = await mediator.Send(new ObterDadosResumidosReponsavelPorCpfQuery(solicitarReiniciarSenhaDto.Cpf));

            if (usuario.PrimeiroAcesso == true)
                throw new NegocioException($"O usuário {Formatacao.FormatarCpf(solicitarReiniciarSenhaDto.Cpf)} - {usuarioEol.Nome} deverá informar a data de nascimento de um dos estudantes que é responsável no campo de senha!");

            await mediator.Send(new ReiniciarSenhaCommand() { Id = usuario.Id, PrimeiroAcesso = true });
            var mensagemSucesso = $"A senha do usuário {Formatacao.FormatarCpf(usuario.Cpf)} - {usuarioEol.Nome} foi reiniciada com sucesso. No próximo acesso ao aplicativo o usuário deverá informar a data de nascimento de um dos estudantes que é responsável!";
            return RespostaApi.Sucesso(mensagemSucesso);
        }

    }
}
