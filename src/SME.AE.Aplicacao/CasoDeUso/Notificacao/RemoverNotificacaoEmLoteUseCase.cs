using MediatR;
using SME.AE.Aplicacao.Comandos.Notificacao.Remover;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Interfaces;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class RemoverNotificacaoEmLoteUseCase : AbstractUseCase, IRemoverNotificacaoEmLoteUseCase
    {
        public RemoverNotificacaoEmLoteUseCase(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RespostaApi> Executar(long[] id)
        {
            RespostaApi resposta = new RespostaApi();            

            var removeuNotificaoUsuarios = await mediator
                .Send(new RemoverNotificacaoUsuarioCommand(id));

            await mediator.Send(new RemoverNotificacaoAlunoPorNotificacoesIdsCommand(id));

            await mediator.Send(new RemoverNotificacaoTurmaPorNotificacoesIdsCommand(id));

            if (!removeuNotificaoUsuarios)
            {
                resposta.Erros.SetValue($"Erro ao excluir Registros de leitura", 0);
                return resposta;
            }

            resposta.Erros = await mediator
                .Send(new RemoverNotificacaoCommand(id));

            resposta.Ok = resposta.Erros?[0] == null;

            return resposta;
        }
    }
}
