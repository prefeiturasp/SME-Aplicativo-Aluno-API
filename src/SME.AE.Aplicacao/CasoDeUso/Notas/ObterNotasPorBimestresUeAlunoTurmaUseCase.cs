using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;
using SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterNotasPorBimestresUeAlunoTurmaUseCase : IObterNotasPorBimestresUeAlunoTurmaUseCase
    {
        private readonly IMediator mediator;

        public ObterNotasPorBimestresUeAlunoTurmaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponenteDto>> Executar(AlunoBimestresTurmaDto notaAlunoDto)
        {
            var notasConceitosBimestreComponente = await mediator.Send(new ObterNotasPorBimestresUeAlunoTurmaQuery(notaAlunoDto.Bimestres,
                                                                                                                   notaAlunoDto.TurmaCodigo,
                                                                                                                   notaAlunoDto.UeCodigo,
                                                                                                                   notaAlunoDto.AlunoCodigo));
            var notaAlunoCores = await mediator.Send(new ObterNotaAlunoCoresQuery());

            foreach (var notaConceito in notasConceitosBimestreComponente)
            {
                notaConceito.ComponenteCurricularNome = await mediator.Send(new ObterNomeComponenteCurricularQuery(notaConceito.ComponenteCurricularCodigo));
                notaConceito.CorDaNota = decimal.TryParse(notaConceito.NotaConceito, out var notaEmValor)
                    ? DefinirCorDaNotaPorValor(notaEmValor, notaAlunoCores)
                    : DefinirCorDaNotaPorConceito(notaConceito.NotaConceito, notaAlunoCores);
            }

            return notasConceitosBimestreComponente;
        }
        private string DefinirCorDaNotaPorValor(decimal nota, IEnumerable<NotaAlunoCor> notaAlunoCores)
        {
            string cor = null;
            switch (nota)
            {
                case decimal n when (n < 5.00m):
                    cor = notaAlunoCores.FirstOrDefault(x => x.Nota == NotaAlunoCor.NotaAbaixo5)?.Cor;
                    break;

                case decimal n when (n <= 6.99m && n >= 5.00m):
                    cor = notaAlunoCores.FirstOrDefault(x => x.Nota == NotaAlunoCor.NotaEntre7e5)?.Cor;
                    break;

                case decimal n when (n >= 7.00m):
                    cor = notaAlunoCores.FirstOrDefault(x => x.Nota == NotaAlunoCor.NotaAcimaDe7)?.Cor;
                    break;
            }

            return cor ?? NotaAlunoCor.CorPadrao;
        }

        private string DefinirCorDaNotaPorConceito(string conceito, IEnumerable<NotaAlunoCor> notaAlunoCores)
        {
            var notaAlunoCor = notaAlunoCores.FirstOrDefault(x => x.Nota.ToUpper() == conceito.ToUpper());
            return notaAlunoCor?.Cor ?? NotaAlunoCor.CorPadrao;
        }
    }
}
