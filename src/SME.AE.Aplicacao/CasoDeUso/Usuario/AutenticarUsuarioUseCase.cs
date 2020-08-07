using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using SME.AE.Aplicacao.Comandos.Token.Criar;
using SME.AE.Aplicacao.Comandos.Usuario.InseriDispositivo;
using SME.AE.Aplicacao.Comum.Excecoes;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;

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
            var resposta = await mediator.Send(new AutenticarUsuarioCommand(cpf, senha));

            if (!resposta.Ok)
                throw new NegocioException(resposta.Erros.Join());
            
            var token = await mediator.Send(new CriarTokenCommand(cpf));
            await mediator.Send(new UsuarioDispositivoCommand(cpf, dispositivoId));

            var data = ((RespostaAutenticar)resposta.Data);
            data.Token = token;
            resposta.Data = data;

            return resposta;
        }
    }
}