using MediatR;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Aluno
{
   public class DadosDoAlunoUseCase
    {
        public static async Task<IEnumerable<Dominio.Entidades.Aluno>> Executar(IMediator mediator, string cpf)
        {
            return await mediator.Send(new DadosAlunoComando(cpf));
        }
    }
}
