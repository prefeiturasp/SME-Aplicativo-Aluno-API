using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.TesteArquitetura
{
    public interface ITesteArquiteturaUseCase
    {
        Task<RespostaApi> Executar(IMediator mediator);
    }
}
