using Microsoft.Practices.ObjectBuilder2;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class TransferirNotaSgpCasoDeUso
    {
        private readonly INotaAlunoRepositorio notaAlunoRepositorio;
        private readonly INotaAlunoSgpRepositorio notaAlunoSgpRepositorio;
        private readonly IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio;

        public TransferirNotaSgpCasoDeUso(INotaAlunoRepositorio notaAlunoRepositorio, INotaAlunoSgpRepositorio notaAlunoSgpRepositorio, IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio)
        {
            this.notaAlunoRepositorio = notaAlunoRepositorio ?? throw new ArgumentNullException(nameof(notaAlunoRepositorio));
            this.notaAlunoSgpRepositorio = notaAlunoSgpRepositorio ?? throw new ArgumentNullException(nameof(notaAlunoSgpRepositorio));
            this.workerProcessoAtualizacaoRepositorio = workerProcessoAtualizacaoRepositorio ?? throw new ArgumentNullException(nameof(workerProcessoAtualizacaoRepositorio));
        }

        public async Task ExecutarAsync()
        {
            var desdeAnoLetivo = DateTime.Today.Year - 1; // esse e o anterior procaso de mudanca de 4o bimestre.

            var notaAlunoSgp = await notaAlunoSgpRepositorio.ObterNotaAlunoSgp(desdeAnoLetivo);
            await notaAlunoRepositorio.SalvarNotaAlunosBatch(notaAlunoSgp);

            var notaAlunoAE = await notaAlunoRepositorio.ObterListaParaExclusao(desdeAnoLetivo);
            await RemoverExcetoSgp(notaAlunoSgp, notaAlunoAE);
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

            await Task.Run(() =>
                notaAlunoSobrando
                    .ForEach(async notaExcluir => await notaAlunoRepositorio.ExcluirNotaAluno(notaExcluir))
            );
        }
    }
}
