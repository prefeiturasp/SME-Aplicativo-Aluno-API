using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comandos.Notificacao.Remover;
using SME.AE.Aplicacao.Comum.Modelos;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class RemoverNotificacaoEmLoteUseCase
    {
        public static async Task<RespostaApi> Executar(IMediator mediator, long[] id)
        {
            RespostaApi resposta = new RespostaApi();
            resposta.Ok = true;
            var erros = await mediator.Send(new RemoverNotificacaoCommand(id));
            erros.Trim();
            if (erros.Length > 1)
            {
                resposta.Ok = false;
                resposta.Erros.SetValue(erros, 1);
                return resposta;
            }

            return resposta;
        }
    }
}