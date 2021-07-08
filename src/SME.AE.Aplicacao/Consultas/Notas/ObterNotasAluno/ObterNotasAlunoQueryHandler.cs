using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;
using SME.AE.Comum.Excecoes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.Notas
{
    public class ObterNotasAlunoQueryHandler : IRequestHandler<ObterNotasAlunoQuery, NotaAlunoPorBimestreResposta>
    {
        private readonly INotaAlunoRepositorio _notaAlunoRepositorio;
        private readonly INotaAlunoCorRepositorio _notaAlunoCorRepositorio;

        public ObterNotasAlunoQueryHandler(INotaAlunoRepositorio notaAlunoRepositorio, INotaAlunoCorRepositorio notaAlunoCorRepositorio)
        {
            _notaAlunoRepositorio = notaAlunoRepositorio ?? throw new System.ArgumentNullException(nameof(notaAlunoRepositorio));
            _notaAlunoCorRepositorio = notaAlunoCorRepositorio ?? throw new System.ArgumentNullException(nameof(notaAlunoCorRepositorio));
        }

        public async Task<NotaAlunoPorBimestreResposta> Handle(ObterNotasAlunoQuery request, CancellationToken cancellationToken)
        {
            var notaAlunoPorBimestreResposta = _notaAlunoRepositorio.ObterNotasAluno(request.AnoLetivo, request.Bimestre, request.CodigoUe, request.CodigoTurma, request.CodigoAluno);
            var notaAlunoCor = _notaAlunoCorRepositorio.ObterAsync();
            await Task.WhenAll(notaAlunoPorBimestreResposta, notaAlunoCor);

            var notasAlunoPorBimestreRespostaConsolidado = notaAlunoPorBimestreResposta.Result;
            if (notasAlunoPorBimestreRespostaConsolidado is null)
            {
                throw new NegocioException("Não foi possível obter as notas do aluno.");
            }

            DefinirCoresDasNotas(notasAlunoPorBimestreRespostaConsolidado, notaAlunoCor.Result);
            return notasAlunoPorBimestreRespostaConsolidado;
        }

        private void DefinirCoresDasNotas(NotaAlunoPorBimestreResposta notasAlunoPorBimestreResposta, IEnumerable<NotaAlunoCor> notaAlunoCores)
        {
            if (notasAlunoPorBimestreResposta.Bimestre != NotaAlunoPorBimestreResposta.BimestreDeFechamento || (!notaAlunoCores?.Any() ?? true))
            {
                foreach (var notaAluno in notasAlunoPorBimestreResposta.NotasPorComponenteCurricular)
                {
                    notaAluno.CorNotaAluno = NotaAlunoCor.CorPadrao;
                }
                return;
            }

            foreach (var notaAluno in notasAlunoPorBimestreResposta.NotasPorComponenteCurricular)
            {
                notaAluno.CorNotaAluno = decimal.TryParse(notaAluno.Nota, out var notaEmValor)
                    ? DefinirCorDaNotaPorValor(notaEmValor, notaAlunoCores)
                    : DefinirCorDaNotaPorConceito(notaAluno.Nota, notaAlunoCores);
            }
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