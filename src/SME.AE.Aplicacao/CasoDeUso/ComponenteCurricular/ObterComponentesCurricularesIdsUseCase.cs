using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterComponentesCurricularesIdsUseCase : IObterComponentesCurricularesIdsUseCase
    {
        private readonly IMediator mediator;

        public ObterComponentesCurricularesIdsUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<IEnumerable<ComponenteCurricularDto>> Executar(AlunoBimestresTurmaDto notaAlunoDto)
        {
            var notasConceitosBimestreComponente = await mediator.Send(new ObterNotasPorBimestresUeAlunoTurmaQuery(notaAlunoDto.Bimestres,
                                                                                                                  notaAlunoDto.TurmaCodigo,
                                                                                                                  notaAlunoDto.UeCodigo,
                                                                                                                  notaAlunoDto.AlunoCodigo));
            var componentes = notasConceitosBimestreComponente.Select(a => new ComponenteCurricularDto(a.ComponenteCurricularCodigo, a.ComponenteCurricularNome)).ToList();
            return componentes.Distinct();
        }
    }
}
