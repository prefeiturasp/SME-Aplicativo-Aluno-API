using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.Frequencia;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class ObterFrequenciaAlunoUseCase : IObterFrequenciaAlunoUseCase
    {
        private readonly IMediator mediator;

        public ObterFrequenciaAlunoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<FrequenciaAlunoResposta>> Executar(string codigoUe, long codigoTurma, string codigoAluno)
        {
            return await mediator.Send(new ObterFrequenciaAlunoQuery(codigoUe, codigoTurma, codigoAluno));
        }


    }
}