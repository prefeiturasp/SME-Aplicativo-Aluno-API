using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterNotasPorBimestresUeAlunoTurmaUseCase : IObterNotasPorBimestresUeAlunoTurmaUseCase
    {
        private readonly IMediator mediator;

        public ObterNotasPorBimestresUeAlunoTurmaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponenteDto>> Executar(NotaConceitoPorBimestresAlunoTurmaDto notaAlunoDto)
        {
            var notasConceitosBimestreComponente = await mediator.Send(new ObterNotasPorBimestresUeAlunoTurmaQuery(notaAlunoDto.Bimestres,
                                                                                                                   notaAlunoDto.TurmaId,
                                                                                                                   notaAlunoDto.UeId,
                                                                                                                   notaAlunoDto.AlunoCodigo));

            return notasConceitosBimestreComponente;
        }
    }
}
