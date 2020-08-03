using MediatR;
using SME.AE.Aplicacao.Comandos.TesteArquitetura;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.TesteArquitetura
{
    public class TesteArquiteturaUseCase : ITesteArquiteturaUseCase
    {
        public async Task<RespostaApi> Executar(IMediator mediator)
        {
            await mediator.Send(new TesteArquiteturaCommand());

            return RespostaApi.Sucesso();
        }
    }
}
