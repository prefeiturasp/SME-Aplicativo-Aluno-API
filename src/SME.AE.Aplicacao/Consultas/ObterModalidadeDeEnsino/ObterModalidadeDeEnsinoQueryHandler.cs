using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterModalidadeDeEnsinoQueryHandler : IRequestHandler<ObterModalidadeDeEnsinoQuery, TurmaModalidadeDeEnsinoDto>
    {
        private readonly ITurmaRepositorio turmaRepositorio;

        public ObterModalidadeDeEnsinoQueryHandler(ITurmaRepositorio turmaRepositorio)
        {
            this.turmaRepositorio = turmaRepositorio ?? throw new System.ArgumentNullException(nameof(turmaRepositorio));
        }

        public async Task<TurmaModalidadeDeEnsinoDto> Handle(ObterModalidadeDeEnsinoQuery request, CancellationToken cancellationToken)
        {
            return await turmaRepositorio.ObterModalidadeDeEnsino(request.CodigoTurma);
        }
    }
}
