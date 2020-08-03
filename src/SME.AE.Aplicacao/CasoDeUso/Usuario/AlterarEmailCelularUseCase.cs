using MediatR;
using SME.AE.Aplicacao.Comandos.Usuario.AlterarEmailCelular;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class AlterarEmailCelularUseCase : IAlterarEmailCelularUseCase
    {
        public async Task<RespostaApi> Executar(IMediator mediator, AlterarEmailCelularDto alterarEmailCelularDto)
        {
            await mediator.Send(new AlterarEmailCelularCommand(alterarEmailCelularDto));

            return RespostaApi.Sucesso();
        }
    }
}
