using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.Notas
{
    public class ObterNotasAlunoQueryHandler : IRequestHandler<ObterNotasAlunoQuery, IEnumerable<NotaAlunoResposta>>
    {
        private readonly INotaAlunoRepositorio _notaAlunoRepositorio;

        public ObterNotasAlunoQueryHandler(INotaAlunoRepositorio notaAlunoRepositorio)
        {
            _notaAlunoRepositorio = notaAlunoRepositorio ?? throw new System.ArgumentNullException(nameof(notaAlunoRepositorio));
        }

        public async Task<IEnumerable<NotaAlunoResposta>> Handle(ObterNotasAlunoQuery request, CancellationToken cancellationToken)
        {
            return await _notaAlunoRepositorio.ObterNotasAluno(request.AnoLetivo, request.CodigoUe, request.CodigoTurma, request.CodigoAluno);
        }
    }
}
