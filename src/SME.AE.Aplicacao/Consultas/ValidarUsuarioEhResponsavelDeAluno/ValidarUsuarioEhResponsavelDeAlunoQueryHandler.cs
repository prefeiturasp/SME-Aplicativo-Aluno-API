using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas
{
    public class ValidarUsuarioEhResponsavelDeAlunoQueryHandler : IRequestHandler<ValidarUsuarioEhResponsavelDeAlunoQuery, bool>
    {
        private readonly IMediator mediator;

        public ValidarUsuarioEhResponsavelDeAlunoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(ValidarUsuarioEhResponsavelDeAlunoQuery request, CancellationToken cancellationToken)
        {
            var alunosDoResponsavel = await mediator.Send(new ObterDadosResponsaveisQuery(request.Cpf));

            if (alunosDoResponsavel == null || !alunosDoResponsavel.Any())
                return false;
            return true;
        }
    }
}
