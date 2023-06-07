﻿using Polly;
using Polly.Registry;
using RabbitMQ.Client;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class TransferirNotaSgpCasoDeUso : ITransferirNotaSgpCasoDeUso
    {
        private readonly INotaAlunoRepositorio notaAlunoRepositorio;
        private readonly INotaAlunoSgpRepositorio notaAlunoSgpRepositorio;
        private readonly IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio;
        private readonly IUeSgpRepositorio ueSgpRepositorio;
        private readonly IAsyncPolicy policy;
        private readonly ConnectionFactory connectionFactory;

        public TransferirNotaSgpCasoDeUso(INotaAlunoRepositorio notaAlunoRepositorio,
                                          INotaAlunoSgpRepositorio notaAlunoSgpRepositorio,
                                          IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio,
                                          IUeSgpRepositorio ueSgpRepositorio,
                                          IReadOnlyPolicyRegistry<string> registry, 
                                          ConnectionFactory connectionFactory)
        {
            this.notaAlunoRepositorio = notaAlunoRepositorio ?? throw new ArgumentNullException(nameof(notaAlunoRepositorio));
            this.notaAlunoSgpRepositorio = notaAlunoSgpRepositorio ?? throw new ArgumentNullException(nameof(notaAlunoSgpRepositorio));
            this.workerProcessoAtualizacaoRepositorio = workerProcessoAtualizacaoRepositorio ?? throw new ArgumentNullException(nameof(workerProcessoAtualizacaoRepositorio));
            this.ueSgpRepositorio = ueSgpRepositorio ?? throw new ArgumentNullException(nameof(ueSgpRepositorio));
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this.policy = registry.Get<IAsyncPolicy>(PoliticaPolly.PublicaFila);
        }

        public async Task ExecutarAsync()
        {
            var anosLetivosConsiderados = new int[] { DateTime.Today.Year - 1, DateTime.Today.Year }; // esse e o anterior para o caso de mudanca de 4o bimestre.
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
                    var notaAlunoSgp = await notaAlunoSgpRepositorio.ObterNotaAlunoSgp(anoAtual, id);
                    await notaAlunoRepositorio.SalvarNotaAlunosBatch(notaAlunoSgp);

                    var notaAlunoAE = await notaAlunoRepositorio.ObterListaParaExclusao(anoAtual, notaAlunoSgp.FirstOrDefault().CodigoUe);
                    await RemoverExcetoSgp(notaAlunoSgp, notaAlunoAE);
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

            var contador = 1;
            var total = notaAlunoSobrando.Length;
            foreach (var notaExcluir in notaAlunoSobrando)
            {
                await policy.ExecuteAsync(() => notaAlunoRepositorio.ExcluirNotaAluno(notaExcluir));
                Debug.WriteLine($"• • • Excluir nota {contador}/{total} • • •");
                contador++;
            }
        }
    }
}
