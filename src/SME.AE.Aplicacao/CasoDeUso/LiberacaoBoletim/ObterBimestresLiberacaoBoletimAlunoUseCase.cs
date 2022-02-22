using MediatR;
using SME.AE.Aplicacao.Consultas;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterBimestresLiberacaoBoletimAlunoUseCase : IObterBimestresLiberacaoBoletimAlunoUseCase
    {
        private readonly IMediator mediator;

        public ObterBimestresLiberacaoBoletimAlunoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<int[]> Executar(string turmaCodigo)
        {
            return await mediator.Send(new ObterBimestresLiberacaoBoletimQuery(turmaCodigo));
        }
    }
}

