using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.Notas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class ObterNotasAlunoUseCase : IObterNotasAlunoUseCase
    {
        private readonly IMediator mediator;

        public ObterNotasAlunoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<NotaAlunoResposta>> Executar(int anoLetivo, string codigoUe, string codigoTurma, string codigoAluno)
        {
            return await mediator.Send(new ObterNotasAlunoQuery(anoLetivo, codigoUe, codigoTurma, codigoAluno));
        }
    }
}