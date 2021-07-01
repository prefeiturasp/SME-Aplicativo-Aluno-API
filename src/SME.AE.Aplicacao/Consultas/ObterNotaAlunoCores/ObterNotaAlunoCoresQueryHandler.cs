using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterNotaAlunoCoresQueryHandler : IRequestHandler<ObterNotaAlunoCoresQuery, IEnumerable<NotaAlunoCor>>
    {
        private readonly INotaAlunoCorRepositorio notaAlunoCorRepositorio;

        public ObterNotaAlunoCoresQueryHandler(INotaAlunoCorRepositorio notaAlunoCorRepositorio)
        {
            this.notaAlunoCorRepositorio = notaAlunoCorRepositorio ?? throw new System.ArgumentNullException(nameof(notaAlunoCorRepositorio));
        }

        public async Task<IEnumerable<NotaAlunoCor>> Handle(ObterNotaAlunoCoresQuery request, CancellationToken cancellationToken)
        {
            return await notaAlunoCorRepositorio.ObterAsync();
        }
    }
}
