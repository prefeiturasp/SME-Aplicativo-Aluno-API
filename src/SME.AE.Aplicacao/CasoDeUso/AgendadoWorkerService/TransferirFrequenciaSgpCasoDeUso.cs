using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class TransferirFrequenciaSgpCasoDeUso : ITransferirFrequenciaSgpCasoDeUso
    {
        private readonly IFrequenciaAlunoRepositorio frequenciaAlunoRepositorio;
        private readonly IFrequenciaAlunoSgpRepositorio frequenciaAlunoSgpRepositorio;
        private readonly IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio;
        private readonly IUeSgpRepositorio ueSgpRepositorio;

        public TransferirFrequenciaSgpCasoDeUso(IFrequenciaAlunoRepositorio frequenciaAlunoRepositorio,
                                                IFrequenciaAlunoSgpRepositorio frequenciaAlunoSgpRepositorio,
                                                IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio,
                                                IUeSgpRepositorio ueSgpRepositorio)
        {
            this.frequenciaAlunoRepositorio = frequenciaAlunoRepositorio ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepositorio));
            this.frequenciaAlunoSgpRepositorio = frequenciaAlunoSgpRepositorio ?? throw new ArgumentNullException(nameof(frequenciaAlunoSgpRepositorio));
            this.workerProcessoAtualizacaoRepositorio = workerProcessoAtualizacaoRepositorio ?? throw new ArgumentNullException(nameof(workerProcessoAtualizacaoRepositorio));
            this.ueSgpRepositorio = ueSgpRepositorio ?? throw new ArgumentNullException(nameof(ueSgpRepositorio));
        }

        public async Task ExecutarAsync()
        {
            var anosConsiderados = new int[] { DateTime.Today.Year - 1, DateTime.Today.Year };
            var uesId = await ueSgpRepositorio.ObterIdUes();

            foreach (var anoAtual in anosConsiderados)
            {
                foreach (var ueId in uesId)
                {
                    var frequenciaAlunosSgp =
                        (await frequenciaAlunoSgpRepositorio.ObterFrequenciaAlunoSgp(anoAtual, ueId))
                        .Where(freq => (
                            string.IsNullOrWhiteSpace(freq.CodigoAluno) || !string.IsNullOrWhiteSpace(freq.DiasAusencias)
                    ));
                    await frequenciaAlunoRepositorio.SalvarFrequenciaAlunosBatch(frequenciaAlunosSgp);

                    var frequenciaAlunosAE = await frequenciaAlunoRepositorio.ObterListaParaExclusao(anoAtual);
                    await RemoverExcetoSgp(frequenciaAlunosSgp, frequenciaAlunosAE);                    
                }
            }

            await workerProcessoAtualizacaoRepositorio.IncluiOuAtualizaUltimaAtualizacao("TransferirFrequenciaSgp");
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
                await frequenciaAlunoRepositorio.ExcluirFrequenciaAluno(frequencia);
        }

    }
}
