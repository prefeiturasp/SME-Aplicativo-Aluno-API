using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using SME.AE.Aplicacao.Comandos.Token.Criar;
using SME.AE.Aplicacao.Comandos.Usuario.InseriDispositivo;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class AutenticarUsuarioUseCase
    {
        public static async Task<RespostaApi> Executar(IMediator mediator, string cpf, string senha)
        {
            var resposta = await mediator.Send(new AutenticarUsuarioCommand(cpf, senha));

            string dispositivoId = "2222222";
            if (!resposta.Ok)
                throw new Exception(resposta.Erros.Join());
            
            var token = await mediator.Send(new CriarTokenCommand(cpf));
            var data = ((RespostaAutenticar)resposta.Data);
            // Cria usuarioDispositivoCommand
            var bole = mediator.Send(new UsuarioDispositivoCommand(data.Id, dispositivoId));
            data.Token = token;
            resposta.Data = data;

            return resposta;
        }
    }
}