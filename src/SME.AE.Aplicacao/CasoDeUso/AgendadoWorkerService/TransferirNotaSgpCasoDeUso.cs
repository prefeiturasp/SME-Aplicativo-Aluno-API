using Microsoft.Practices.ObjectBuilder2;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class TransferirNotaSgpCasoDeUso
    {
        private readonly INotaAlunoRepositorio notaAlunoRepositorio;
        private readonly INotaAlunoSgpRepositorio notaAlunoSgpRepositorio;
        private readonly IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio;
        private readonly IUeSgpRepositorio ueSgpRepositorio;

        public TransferirNotaSgpCasoDeUso(INotaAlunoRepositorio notaAlunoRepositorio,
                                          INotaAlunoSgpRepositorio notaAlunoSgpRepositorio,
                                          IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio,
                                          IUeSgpRepositorio ueSgpRepositorio)
        {
            this.notaAlunoRepositorio = notaAlunoRepositorio ?? throw new ArgumentNullException(nameof(notaAlunoRepositorio));
            this.notaAlunoSgpRepositorio = notaAlunoSgpRepositorio ?? throw new ArgumentNullException(nameof(notaAlunoSgpRepositorio));
            this.workerProcessoAtualizacaoRepositorio = workerProcessoAtualizacaoRepositorio ?? throw new ArgumentNullException(nameof(workerProcessoAtualizacaoRepositorio));
            this.ueSgpRepositorio = ueSgpRepositorio ?? throw new ArgumentNullException(nameof(ueSgpRepositorio));
        }

        public async Task ExecutarAsync()
        {
            var anosLetivosConsiderados = new int[] { DateTime.Today.Year - 1, DateTime.Today.Year }; // esse e o anterior para o caso de mudanca de 4o bimestre.
            var idsUes = (await ueSgpRepositorio.ObterIdUes()).ToList();

            foreach (var anoAtual in anosLetivosConsiderados)
            {
                foreach (var id in idsUes)
                {
                    var notaAlunoSgp = await notaAlunoSgpRepositorio.ObterNotaAlunoSgp(anoAtual, id);
                    await notaAlunoRepositorio.SalvarNotaAlunosBatch(notaAlunoSgp);

                    var notaAlunoAE = await notaAlunoRepositorio.ObterListaParaExclusao(anoAtual);
                    await RemoverExcetoSgp(notaAlunoSgp, notaAlunoAE);
                    Debug.WriteLine($"• • • UE Id: {id} concluída • • •");
                }                
            }

            await workerProcessoAtualizacaoRepositorio.IncluiOuAtualizaUltimaAtualizacao("TransferirNotaSgp");
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

            //await Task.Run(() =>
            //    notaAlunoSobrando
            //        .ForEach(async notaExcluir => await notaAlunoRepositorio.ExcluirNotaAluno(notaExcluir))
            //);

            foreach (var notaExcluir in notaAlunoSobrando)
                await notaAlunoRepositorio.ExcluirNotaAluno(notaExcluir);
        }
    }
}
