using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Frequencia;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using SME.AE.Aplicacao.Consultas.Frequencia;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Frequencia
{
    public class ObterFrequenciaAlunoUseCase : IObterFrequenciaAlunoUseCase
    {
        private readonly IMediator _mediator;

        public ObterFrequenciaAlunoUseCase(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<FrequenciaAlunoResposta> Executar(ObterFrequenciaAlunoDto frequenciaAlunoDto)
        {
            return await _mediator.Send(new ObterFrequenciaAlunoQuery(frequenciaAlunoDto.AnoLetivo, frequenciaAlunoDto.CodigoUe, frequenciaAlunoDto.CodigoTurma, frequenciaAlunoDto.CodigoAluno));
        }
    }
}