using SME.AE.Aplicacao.Comum.Enumeradores;
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
            var notaAlunoSgp = await notaAlunoSgpRepositorio.ObterNotaAlunoSgp();
            await notaAlunoRepositorio.SalvarNotaAlunosBatch(notaAlunoSgp);
            await workerProcessoAtualizacaoRepositorio.IncluiOuAtualizaUltimaAtualizacao("TransferirNotaSgp");
        }
    }
}
