using MediatR;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterEventosAlunoPorMesUseCase : IObterEventosAlunoPorMesUseCase
    {
        private readonly IMediator mediator;

        public ObterEventosAlunoPorMesUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }
        public async Task<IEnumerable<EventoRespostaDto>> Executar(string cpf, long codigoAluno, int mes, int ano)
        {
            var eventos = await mediator.Send(new ObterEventosAlunoPorMesQuery { Cpf = cpf, CodigoAluno = codigoAluno, MesAno = new DateTime(ano, mes, 1)});

            return eventos;
        }
    }
}
