using Google.Apis.Logging;
using MediatR;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using RabbitMQ.Client;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class TransferirFrequenciaSgpCasoDeUso : ITransferirFrequenciaSgpCasoDeUso
    {
        private const int QUANTIDADE_REGISTROS_POR_PAGINA = 1000;

        private readonly IFrequenciaAlunoRepositorio frequenciaAlunoRepositorio;
        private readonly IFrequenciaAlunoSgpRepositorio frequenciaAlunoSgpRepositorio;
        private readonly IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio;
        private readonly IUeSgpRepositorio ueSgpRepositorio;
        private readonly IAsyncPolicy policy;
        private readonly ConnectionFactory connectionFactory;
        private readonly ILogger<TransferirFrequenciaSgpCasoDeUso> logger;
        private readonly IMediator mediator;

        public TransferirFrequenciaSgpCasoDeUso(IFrequenciaAlunoRepositorio frequenciaAlunoRepositorio,
                                                IFrequenciaAlunoSgpRepositorio frequenciaAlunoSgpRepositorio,
                                                IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio,
                                                IUeSgpRepositorio ueSgpRepositorio,
                                                IReadOnlyPolicyRegistry<string> registry,
                                                ConnectionFactory connectionFactory,
                                                ILogger<TransferirFrequenciaSgpCasoDeUso> logger,
                                                IMediator mediator)
        {
            this.frequenciaAlunoRepositorio = frequenciaAlunoRepositorio ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepositorio));
            this.frequenciaAlunoSgpRepositorio = frequenciaAlunoSgpRepositorio ?? throw new ArgumentNullException(nameof(frequenciaAlunoSgpRepositorio));
            this.workerProcessoAtualizacaoRepositorio = workerProcessoAtualizacaoRepositorio ?? throw new ArgumentNullException(nameof(workerProcessoAtualizacaoRepositorio));
            this.ueSgpRepositorio = ueSgpRepositorio ?? throw new ArgumentNullException(nameof(ueSgpRepositorio));
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.policy = registry.Get<IAsyncPolicy>(PoliticaPolly.PublicaFila);
        }

        public async Task ExecutarAsync()
        {
            var parametrosAnosConsiderados = await mediator.Send(new ObterParametrosSistemaPorChavesQuery(new string[] { "TransferirFrequenciaSgpAnosConsiderados" }));
            var anosConsiderados = parametrosAnosConsiderados.FirstOrDefault()?.Conteudo.Split(",").Select(c => int.Parse(c)).ToArray() ?? new int[] { DateTime.Now.Year };
            var uesId = await ueSgpRepositorio.ObterIdUes();
            await Executar(anosConsiderados, uesId);
            await workerProcessoAtualizacaoRepositorio.IncluiOuAtualizaUltimaAtualizacao("TransferirFrequenciaSgp");
        }

        public async Task ExecutarPorAnoUeAsync(int anoLetivo, long ueId)
        {
            await Executar(new int[] { anoLetivo }, new long[] { ueId });
        }

        private async Task Executar(int[] anosConsiderados, long[] uesId)
        {
            foreach (var anoAtual in anosConsiderados)
            {
                foreach (var ueId in uesId)
                {
                    var contadorPagina = 1;
                    IEnumerable<FrequenciaAlunoSgpDto> frequenciaAlunosSgp;
                    var frequenciaAlunosSgpAcumulado = new List<FrequenciaAlunoSgpDto>();
                    var codigoUe = (string)null;

                    do
                    {
                        frequenciaAlunosSgp = (await frequenciaAlunoSgpRepositorio.ObterFrequenciaAlunoSgp(anoAtual, ueId, contadorPagina, QUANTIDADE_REGISTROS_POR_PAGINA))
                            .Where(freq => (string.IsNullOrWhiteSpace(freq.CodigoAluno) || !string.IsNullOrWhiteSpace(freq.DiasAusencias)));

                        if (string.IsNullOrEmpty(codigoUe))
                            codigoUe = frequenciaAlunosSgp.FirstOrDefault().CodigoUe;

                        await frequenciaAlunoRepositorio
                            .SalvarFrequenciaAlunosBatch(frequenciaAlunosSgp);

                        frequenciaAlunosSgpAcumulado.AddRange(frequenciaAlunosSgp);

                        logger.LogInformation($">>> Salvar Frequência Aluno - Ano Letivo: {anoAtual} / UE: {codigoUe} / página: {contadorPagina} <<<");

                        contadorPagina++;

                    } while (frequenciaAlunosSgp != null && frequenciaAlunosSgp.Any());

                    var frequenciaAlunosAE = await frequenciaAlunoRepositorio
                        .ObterListaParaExclusao(anoAtual, codigoUe);

                    await RemoverExcetoSgp(frequenciaAlunosSgpAcumulado, frequenciaAlunosAE);

                    logger.LogInformation($">>> Excluir Frequência Aluno - Ano Letivo: {anoAtual} / UE: {codigoUe} <<<");
                }
            }
        }

        private async Task RemoverExcetoSgp(IEnumerable<FrequenciaAlunoSgpDto> frequenciaAlunoSgp, IEnumerable<FrequenciaAlunoSgpDto> frequenciaAlunoAE)
        {
            var frequenciaAlunoSobrando =
                frequenciaAlunoAE
                .AsParallel()
                .Where(frequenciaAE =>
                   !frequenciaAlunoSgp
                   .Any(frequenciaSgp =>
                       frequenciaSgp.AnoLetivo == frequenciaAE.AnoLetivo &&
                       frequenciaSgp.CodigoAluno == frequenciaAE.CodigoAluno &&
                       frequenciaSgp.CodigoUe == frequenciaAE.CodigoUe &&
                       frequenciaSgp.CodigoTurma == frequenciaAE.CodigoTurma &&
                       frequenciaSgp.Bimestre == frequenciaAE.Bimestre &&
                       frequenciaSgp.CodigoComponenteCurricular == frequenciaAE.CodigoComponenteCurricular
                   ))
                .ToArray();

            foreach (var frequencia in frequenciaAlunoSobrando)
                await policy.ExecuteAsync(() => frequenciaAlunoRepositorio.ExcluirFrequenciaAluno(frequencia));
        }
    }
}
