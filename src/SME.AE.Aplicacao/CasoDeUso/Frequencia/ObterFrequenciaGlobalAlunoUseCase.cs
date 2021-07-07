using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterFrequenciaGlobalAlunoUseCase : IObterFrequenciaGlobalAlunoUseCase
    {
        private readonly IMediator mediator;

        public ObterFrequenciaGlobalAlunoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<FrequenciaGlobalDto> Executar(FiltroFrequenciaGlobalAlunoDto filtro)
        {
            var frequenciaGlobal = await mediator.Send(new ObterFrequenciaGlobalAlunoQuery(filtro.TurmaCodigo, filtro.AlunoCodigo));

            if (frequenciaGlobal != null)
            {
                var turmaModalidadeDeEnsino = await mediator.Send(new ObterModalidadeDeEnsinoQuery(filtro.TurmaCodigo));
                var parametros = turmaModalidadeDeEnsino.ModalidadeDeEnsino == ModalidadeDeEnsino.Infantil
                ? await mediator.Send(new ObterParametrosSistemaPorChavesQuery(FrequenciaAlunoCor.ObterChavesDosParametrosParaEnsinoInfantil()))
                : await mediator.Send(new ObterParametrosSistemaPorChavesQuery(FrequenciaAlunoCor.ObterChavesDosParametros()));

                var obterFrequenciaAlunoCores = await mediator.Send(new ObterFrequenciaAlunoCorPorParametroQuery(parametros));
                var obterFrequenciaAlunoFaixa = await mediator.Send(new ObterFrequenciaAlunoFaixaPorParametroQuery(parametros));

                frequenciaGlobal.CorDaFrequencia = await mediator.Send(new ObterCorQuery(parametros, frequenciaGlobal.Frequencia,
                    obterFrequenciaAlunoCores, obterFrequenciaAlunoFaixa,
                    turmaModalidadeDeEnsino.ModalidadeDeEnsino));
            }

            return frequenciaGlobal;
        }
    }
}
