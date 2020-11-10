using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum.Excecoes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.Notas
{
    public class ObterNotasAlunoQueryHandler : IRequestHandler<ObterNotasAlunoQuery, IEnumerable<NotaAlunoResposta>>
    {
        private readonly INotaAlunoRepositorio _notaAlunoRepositorio;
        private readonly INotaAlunoCorRepositorio _notaAlunoCorRepositorio;

        public ObterNotasAlunoQueryHandler(INotaAlunoRepositorio notaAlunoRepositorio, INotaAlunoCorRepositorio notaAlunoCorRepositorio)
        {
            _notaAlunoRepositorio = notaAlunoRepositorio ?? throw new System.ArgumentNullException(nameof(notaAlunoRepositorio));
            _notaAlunoCorRepositorio = notaAlunoCorRepositorio ?? throw new System.ArgumentNullException(nameof(notaAlunoCorRepositorio));
        }

        public async Task<IEnumerable<NotaAlunoResposta>> Handle(ObterNotasAlunoQuery request, CancellationToken cancellationToken)
        {
            var notaAlunoResposta = _notaAlunoRepositorio.ObterNotasAluno(request.AnoLetivo, request.Bimestre, request.CodigoUe, request.CodigoTurma, request.CodigoAluno);
            var notaAlunoCor = _notaAlunoCorRepositorio.ObterAsync();
            await Task.WhenAll(notaAlunoResposta, notaAlunoCor);

            var notasAlunoRespostaConsolidado = notaAlunoResposta.Result;
            if (notasAlunoRespostaConsolidado is null)
            {
                throw new NegocioException("Não foi possível obter as notas do aluno.");
            }

            DefinirCoresDasNotas(notasAlunoRespostaConsolidado, notaAlunoCor.Result);
            return notasAlunoRespostaConsolidado;
        }

        private void DefinirCoresDasNotas(IEnumerable<NotaAlunoResposta> notasAlunoRespostas, IEnumerable<NotaAlunoCor> notaAlunoCores)
        {
            if (!notaAlunoCores?.Any() ?? true)
            {
                foreach (var notaAluno in notasAlunoRespostas)
                {
                    notaAluno.CorNotaAluno = NotaAlunoCor.CorPadrao;
                }
                return;
            }

            foreach (var notaAluno in notasAlunoRespostas)
            {
                notaAluno.CorNotaAluno = int.TryParse(notaAluno.Nota, out var notaEmValor)
                    ? DefinirCorDaNotaPorValor(notaEmValor, notaAlunoCores)
                    : DefinirCorDaNotaPorConceito(notaAluno.Nota, notaAlunoCores);
            }
        }

        private string DefinirCorDaNotaPorValor(int nota, IEnumerable<NotaAlunoCor> notaAlunoCores)
        {
            string cor = null;
            switch (nota)
            {
                case int n when (n < 5):
                    cor = notaAlunoCores.FirstOrDefault(x => x.Nota == NotaAlunoCor.NotaAbaixo5)?.Cor;
                    break;

                case int n when (n <= 7 && n >= 5):
                    cor = notaAlunoCores.FirstOrDefault(x => x.Nota == NotaAlunoCor.NotaEntre7e5)?.Cor;
                    break;

                case int n when (n > 7):
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