using MediatR;
using SME.AE.Aplicacao.Comandos.Autenticacao;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class LogoutUsuarioUseCase
    {
        public static async Task<RespostaApi> Executar(IMediator mediator, string cpf, string dispositivoId)
        {
            var respostaApi = new RespostaApi();
            respostaApi.Ok = await mediator.Send(new RemoveUsuarioDispositivoCommand(cpf, dispositivoId));
            return respostaApi;
        }
    }

}
