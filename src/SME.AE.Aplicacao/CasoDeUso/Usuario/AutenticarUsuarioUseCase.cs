using MediatR;
using Sentry;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using SME.AE.Aplicacao.Comandos.Logs;
using SME.AE.Aplicacao.Comandos.Token.Criar;
using SME.AE.Aplicacao.Comandos.Usuario.InseriDispositivo;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class AutenticarUsuarioUseCase : IAutenticarUsuarioUseCase
    {
        private readonly IMediator mediator;

        public AutenticarUsuarioUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RespostaApi> Executar(string cpf, string senha, string dispositivoId)
        {
            try
            {
                var resposta = await mediator.Send(new AutenticarUsuarioCommand(cpf, senha));

                if (!resposta.Ok)
                    throw new NegocioException(string.Join("", resposta.Erros));

                var token = await mediator.Send(new CriarTokenCommand(cpf));
                await mediator.Send(new UsuarioDispositivoCommand(cpf, dispositivoId));

                var data = ((RespostaAutenticar)resposta.Data);
                data.Token = token;
                resposta.Data = data;

                return resposta;
            }
            catch (Exception ex)
            {
                var tags = new Dictionary<string, string> { { "CPF", cpf }, { "Senha", senha } };
                var mensagem = $"Não foi possivel realizar do usuario: {tags} , {ex.Message}, {ex.StackTrace} {ex.InnerException} {ex}";
                await mediator.Send(new SalvarLogErroCommand(ex, mensagem, tags));
                return RespostaApi.Falha(ex.Message);
            }
        }
    }
}