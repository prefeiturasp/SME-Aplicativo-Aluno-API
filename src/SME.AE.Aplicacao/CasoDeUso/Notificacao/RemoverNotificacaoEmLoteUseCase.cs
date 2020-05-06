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

           var  removeuNotificaoUsuarios = await mediator.Send(new RemoverNotificacaoUsuarioCommand(id));
            if (removeuNotificaoUsuarios)
            {
                resposta.Erros = await mediator.Send(new RemoverNotificacaoCommand(id));
              
            }

            else
            {
                resposta.Erros.SetValue($"Errro ao excluir Registros de leitura", 0);
            }

            if (resposta.Erros[0] != null)
            {
                resposta.Ok = false;
            }

            return resposta;
        }
    }
}