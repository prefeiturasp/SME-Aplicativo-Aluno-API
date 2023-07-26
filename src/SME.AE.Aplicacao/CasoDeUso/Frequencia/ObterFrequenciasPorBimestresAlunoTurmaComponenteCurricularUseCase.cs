using MediatR;
using SME.AE.Aplicacao.Comum;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var notasConceitosBimestreComponente = await mediator.Send(new ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQuery(dto.Bimestres, dto.TurmaCodigo, dto.AlunoCodigo, dto.ComponenteCurricularId));

            if (notasConceitosBimestreComponente != null)
            {
                var turmaModalidadeDeEnsino = await mediator.Send(new ObterTurmasModalidadesPorCodigosQuery(new string[] { dto.TurmaCodigo }));
                var modalidadeDeEnsino = (ModalidadeDeEnsino)turmaModalidadeDeEnsino.FirstOrDefault().ModalidadeCodigo;

                var parametros = modalidadeDeEnsino == ModalidadeDeEnsino.Infantil
                ? await mediator.Send(new ObterParametrosSistemaPorChavesQuery(FrequenciaAlunoCor.ObterChavesDosParametrosParaEnsinoInfantil()))
                : await mediator.Send(new ObterParametrosSistemaPorChavesQuery(FrequenciaAlunoCor.ObterChavesDosParametros()));

                var obterFrequenciaAlunoCores = await mediator.Send(new ObterFrequenciaAlunoCorPorParametroQuery(parametros));
                var obterFrequenciaAlunoFaixa = await mediator.Send(new ObterFrequenciaAlunoFaixaPorParametroQuery(parametros));

                List<FrequenciaAlunoDto> frequenciasAtualizada = new List<FrequenciaAlunoDto>();
                foreach (var frequencia in notasConceitosBimestreComponente)
                {
                    frequencia.CorDaFrequencia = await mediator.Send(new ObterCorQuery(parametros, frequencia.PercentualFrequencia,
                    obterFrequenciaAlunoCores, obterFrequenciaAlunoFaixa,
                    modalidadeDeEnsino));
                    frequenciasAtualizada.Add(frequencia);
                }
                return frequenciasAtualizada;
            }

            return notasConceitosBimestreComponente;
        }
    }
}
