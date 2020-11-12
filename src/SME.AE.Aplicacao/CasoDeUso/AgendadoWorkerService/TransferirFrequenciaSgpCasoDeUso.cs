using Microsoft.Practices.ObjectBuilder2;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class TransferirFrequenciaSgpCasoDeUso
    {
        private readonly IFrequenciaAlunoRepositorio frequenciaAlunoRepositorio;
        private readonly IFrequenciaAlunoSgpRepositorio frequenciaAlunoSgpRepositorio;
        private readonly IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio;

        public TransferirFrequenciaSgpCasoDeUso(IFrequenciaAlunoRepositorio frequenciaAlunoRepositorio, IFrequenciaAlunoSgpRepositorio frequenciaAlunoSgpRepositorio, IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio)
        {
            this.frequenciaAlunoRepositorio = frequenciaAlunoRepositorio ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepositorio));
            this.frequenciaAlunoSgpRepositorio = frequenciaAlunoSgpRepositorio ?? throw new ArgumentNullException(nameof(frequenciaAlunoSgpRepositorio));
            this.workerProcessoAtualizacaoRepositorio = workerProcessoAtualizacaoRepositorio ?? throw new ArgumentNullException(nameof(workerProcessoAtualizacaoRepositorio));
        }

        public async Task ExecutarAsync()
        {
            var desdeAnoLetivo = DateTime.Today.Year - 1;
            var frequenciaAlunosSgp = 
                (await frequenciaAlunoSgpRepositorio.ObterFrequenciaAlunoSgp(desdeAnoLetivo))
                .Where(freq => (
                    string.IsNullOrWhiteSpace(freq.CodigoAluno) 
                    ||
                    !string.IsNullOrWhiteSpace(freq.DiasAusencias)
                    ));
            await frequenciaAlunoRepositorio.SalvarFrequenciaAlunosBatch(frequenciaAlunosSgp);

            var frequenciaAlunosAE = await frequenciaAlunoRepositorio.ObterListaParaExclusao(desdeAnoLetivo);
            await RemoverExcetoSgp(frequenciaAlunosSgp, frequenciaAlunosAE);
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

            await Task.Run(() =>
                frequenciaAlunoSobrando
                    .ForEach(async frequenciaExcluir => await frequenciaAlunoRepositorio.ExcluirFrequenciaAluno(frequenciaExcluir))
            );
        }

    }
}
