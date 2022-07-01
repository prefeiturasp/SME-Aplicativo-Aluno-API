using MediatR;
using SME.AE.Aplicacao.Comandos.Notificacao.Remover;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class MarcarExcluidaMensagenUsuarioAlunoIdUseCase : IMarcarExcluidaMensagenUsuarioAlunoIdUseCase
    {
        private readonly IMediator mediator;

        public MarcarExcluidaMensagenUsuarioAlunoIdUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(string cpf, long codigoAluno, long id)
        {
            return await mediator
                .Send(new MarcarExcluidaMensagenUsuarioAlunoIdCommand
                {
                    Cpf = cpf,
                    CodigoAluno = codigoAluno,
                    NotificacaoId = id
                });
        }
    }
}
