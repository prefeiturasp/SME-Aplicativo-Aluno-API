using MediatR;
using SME.AE.Aplicacao.Comum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularUseCase : IObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularUseCase
    {
        private readonly IMediator mediator;

        public ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<FrequenciaAlunoDto>> Executar(FrequenciaPorBimestresAlunoTurmaComponenteCurricularDto dto)
        {
            var notasConceitosBimestreComponente = await mediator.Send(new ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQuery(dto.Bimestres,
                                                                                                                  dto.TurmaCodigo,
                                                                                                                  dto.AlunoCodigo,
                                                                                                                  dto.ComponenteCurricularId));

            return notasConceitosBimestreComponente;
        }
    }
}
