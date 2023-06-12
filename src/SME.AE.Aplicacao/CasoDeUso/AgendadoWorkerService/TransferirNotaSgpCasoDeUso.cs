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
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class TransferirNotaSgpCasoDeUso : ITransferirNotaSgpCasoDeUso
    {
        private const int QUANTIDADE_REGISTROS_POR_PAGINA = 1000;

        private readonly INotaAlunoRepositorio notaAlunoRepositorio;
        private readonly INotaAlunoSgpRepositorio notaAlunoSgpRepositorio;
        private readonly IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio;
        private readonly IUeSgpRepositorio ueSgpRepositorio;
        private readonly IAsyncPolicy policy;
        private readonly ConnectionFactory connectionFactory;
        private readonly ILogger<TransferirNotaSgpCasoDeUso> logger;
        private readonly IMediator mediator;

        public TransferirNotaSgpCasoDeUso(INotaAlunoRepositorio notaAlunoRepositorio,
                                          INotaAlunoSgpRepositorio notaAlunoSgpRepositorio,
                                          IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio,
                                          IUeSgpRepositorio ueSgpRepositorio,
                                          IReadOnlyPolicyRegistry<string> registry, 
                                          ConnectionFactory connectionFactory,
                                          ILogger<TransferirNotaSgpCasoDeUso> logger,
                                          IMediator mediator)
        {
            this.notaAlunoRepositorio = notaAlunoRepositorio ?? throw new ArgumentNullException(nameof(notaAlunoRepositorio));
            this.notaAlunoSgpRepositorio = notaAlunoSgpRepositorio ?? throw new ArgumentNullException(nameof(notaAlunoSgpRepositorio));
            this.workerProcessoAtualizacaoRepositorio = workerProcessoAtualizacaoRepositorio ?? throw new ArgumentNullException(nameof(workerProcessoAtualizacaoRepositorio));
            this.ueSgpRepositorio = ueSgpRepositorio ?? throw new ArgumentNullException(nameof(ueSgpRepositorio));
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.policy = registry.Get<IAsyncPolicy>(PoliticaPolly.PublicaFila);
        }

        public async Task ExecutarAsync()
        {
            var parametrosAnosConsiderados = await mediator.Send(new ObterParametrosSistemaPorChavesQuery(new string[] { "TransferirNotaSgpAnosConsiderados" }));
            var anosLetivosConsiderados = parametrosAnosConsiderados.FirstOrDefault()?.Conteudo.Split(",").Select(c => int.Parse(c)).ToArray() ?? new int[] { DateTime.Now.Year };
            var idsUes = (await ueSgpRepositorio.ObterIdUes()).ToList();
            await Executar(anosLetivosConsiderados, idsUes);
            await workerProcessoAtualizacaoRepositorio.IncluiOuAtualizaUltimaAtualizacao("TransferirNotaSgp");
        }

        public async Task ExecutarPorAnoUeAsync(int anoLetivo, long ueId)
        {
            await Executar(new int[] { anoLetivo}, new List<long> { ueId });
        }

        private async Task Executar(int[] anosLetivosConsiderados, List<long> idsUes)
        {
            foreach (var anoAtual in anosLetivosConsiderados)
            {
                foreach (var id in idsUes)
                {
                    var contadorPagina = 1;
                    IEnumerable<NotaAlunoSgpDto> notaAlunoSgp = null;
                    var notaAlunoSgpAcumulado = new List<NotaAlunoSgpDto>();
                    var codigoUe = (string)null;

                    do
                    {                        
                        notaAlunoSgp = await notaAlunoSgpRepositorio
                            .ObterNotaAlunoSgp(anoAtual, id, contadorPagina, QUANTIDADE_REGISTROS_POR_PAGINA);

                        if (string.IsNullOrEmpty(codigoUe))
                            codigoUe = notaAlunoSgp.FirstOrDefault()?.CodigoUe;

                        var notaAlunoSgpAgrupado = notaAlunoSgp
                            .Where(n => !string.IsNullOrWhiteSpace(n.RecomendacoesFamiliaComplementares) &&
                                        !string.IsNullOrWhiteSpace(n.RecomendacoesAlunoComplementares)) 
                            .GroupBy(a => new { a.CodigoTurma, a.Bimestre, a.CodigoAluno, a.CodigoComponenteCurricular })
                            .Select(ag => new NotaAlunoSgpDto()
                            {
                                AnoLetivo = ag.First().AnoLetivo,
                                CodigoUe = ag.First().CodigoUe,
                                CodigoTurma = ag.Key.CodigoTurma,
                                Bimestre = ag.Key.Bimestre,
                                CodigoAluno = ag.Key.CodigoAluno,
                                CodigoComponenteCurricular = ag.Key.CodigoComponenteCurricular,
                                ComponenteCurricular = ag.First().ComponenteCurricular,
                                Nota = ag.First().Nota,
                                NotaDescricao = ag.First().NotaDescricao,
                                RecomendacoesAluno = string.Concat(ag.First().RecomendacoesAluno, string.Join(" ", ag.Select(rfc => rfc.RecomendacoesAlunoComplementares).Distinct())),
                                RecomendacoesFamilia = string.Concat(ag.First().RecomendacoesFamilia, string.Join(" ", ag.Select(rfc => rfc.RecomendacoesFamiliaComplementares).Distinct()))
                            }).Distinct();

                        await notaAlunoRepositorio
                            .SalvarNotaAlunosBatch(notaAlunoSgpAgrupado);

                        notaAlunoSgpAcumulado.AddRange(notaAlunoSgpAgrupado);

                        logger.LogInformation($">>> Salvar Nota Aluno - Ano Letivo: {anoAtual} / UE: {codigoUe} / página: {contadorPagina} <<<");

                        contadorPagina++;
                    } while (notaAlunoSgp != null && notaAlunoSgp.Any());                    

                    var notaAlunoAE = !string.IsNullOrWhiteSpace(codigoUe) ? await notaAlunoRepositorio
                        .ObterListaParaExclusao(anoAtual, codigoUe) : Enumerable.Empty<NotaAlunoSgpDto>();

                    await RemoverExcetoSgp(notaAlunoSgpAcumulado, notaAlunoAE);

                    logger.LogInformation($">>> Excluir Nota Aluno - Ano Letivo: {anoAtual} / UE: {codigoUe} <<<");
                }
            }            
        }

        private async Task RemoverExcetoSgp(IEnumerable<NotaAlunoSgpDto> notaAlunoSgp, IEnumerable<NotaAlunoSgpDto> notaAlunoAE)
        {
            var notaAlunoSobrando =
                notaAlunoAE
                .AsParallel()
                .Where(notaAE =>
                   !notaAlunoSgp
                   .Any(notaSgp =>
                       notaSgp.AnoLetivo == notaAE.AnoLetivo &&
                       notaSgp.CodigoUe == notaAE.CodigoUe &&
                       notaSgp.CodigoTurma == notaAE.CodigoTurma &&
                       notaSgp.CodigoComponenteCurricular == notaAE.CodigoComponenteCurricular &&
                       notaSgp.Bimestre == notaAE.Bimestre
                   ))
                .ToArray();

            foreach (var notaExcluir in notaAlunoSobrando)
                await policy.ExecuteAsync(() => notaAlunoRepositorio.ExcluirNotaAluno(notaExcluir));
        }
    }
}
