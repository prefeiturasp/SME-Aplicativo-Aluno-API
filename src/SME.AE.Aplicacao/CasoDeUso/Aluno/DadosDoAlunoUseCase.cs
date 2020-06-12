using MediatR;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Sentry;
using Xunit.Sdk;

namespace SME.AE.Aplicacao.CasoDeUso.Aluno
{
   public class DadosDoAlunoUseCase
    {
        public static async Task<RespostaApi> Executar(IMediator mediator, string cpf)
        {
            try
            {
                RespostaApi resposta = await mediator.Send(new DadosAlunoCommand(cpf));
                
                if(!resposta.Ok)
                    throw new Exception(resposta.Erros.Join());
                
                return resposta;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}
