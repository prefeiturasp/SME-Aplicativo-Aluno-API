using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.Frequencia
{
    public class ObterFrequenciaAlunoQueryHandler : IRequestHandler<ObterFrequenciaAlunoQuery, IEnumerable<FrequenciaAlunoResposta>>
    {
        private readonly IFrequenciaAlunoRepositorio _frequenciaAlunoRepositorio;

        public ObterFrequenciaAlunoQueryHandler(IFrequenciaAlunoRepositorio frequenciaAlunoRepositorio)
        {
            _frequenciaAlunoRepositorio = frequenciaAlunoRepositorio ?? throw new System.ArgumentNullException(nameof(frequenciaAlunoRepositorio));
        }

        public async Task<IEnumerable<FrequenciaAlunoResposta>> Handle(ObterFrequenciaAlunoQuery request, CancellationToken cancellationToken)
        {
            return await _frequenciaAlunoRepositorio.ObterFrequenciaAluno(request.CodigoUe, request.CodigoTurma, request.CodigoAluno);
        }
    }
}
