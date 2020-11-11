using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;
using SME.AE.Aplicacao.Consultas.Notas;
using SME.AE.Comum.Excecoes;
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

        public async Task<NotaAlunoPorBimestreResposta> Executar(NotaAlunoDto notaAlunoDto)
        {
            if(notaAlunoDto is null)
            {
                throw new NegocioException("Não existem informações de entrada para busca das notas do aluno.");
            }

            var query = new ObterNotasAlunoQuery(notaAlunoDto.AnoLetivo, notaAlunoDto.Bimestre, notaAlunoDto.CodigoUe, notaAlunoDto.CodigoTurma, notaAlunoDto.CodigoAluno);
            return await mediator.Send(query);
        }
    }
}