using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno.PorComponenteCurricular;
using SME.AE.Aplicacao.Consultas.Frequencia.PorComponenteCurricular;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class ObterFrequenciaAlunoPorComponenteCurricularUseCase : IObterFrequenciaAlunoPorComponenteCurricularUseCase
    {
        private readonly IMediator mediator;

        public ObterFrequenciaAlunoPorComponenteCurricularUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<FrequenciaAlunoPorComponenteCurricularResposta> Executar(ObterFrequenciaAlunoPorComponenteCurricularDto frequenciaAlunoDto)
        {
            return await mediator.Send(new ObterFrequenciaAlunoPorComponenteCurricularQuery(frequenciaAlunoDto.AnoLetivo, frequenciaAlunoDto.CodigoUe, frequenciaAlunoDto.CodigoTurma, frequenciaAlunoDto.CodigoAluno, frequenciaAlunoDto.CodigoComponenteCurricular));
        }
    }
}