using MediatR;
using SME.AE.Aplicacao.Comandos.Notificacao.Remover;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class RemoverNotificacaoEmLoteUseCase : IRemoverNotificacaoEmLoteUseCase
    {
        private readonly IMediator mediator;

        public RemoverNotificacaoEmLoteUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<RespostaApi> Executar(long[] id)
        {
            RespostaApi resposta = new RespostaApi();

            var removeuNotificaoUsuarios = await mediator.Send(new RemoverNotificacaoUsuarioCommand(id));

            if (!removeuNotificaoUsuarios)
            {
                resposta.Erros.SetValue($"Erro ao excluir Registros de leitura", 0);
                return resposta;
            }

            resposta.Erros = await mediator.Send(new RemoverNotificacaoCommand(id));

            resposta.Ok = resposta.Erros?[0] == null;

            return resposta;
        }
    }
}
