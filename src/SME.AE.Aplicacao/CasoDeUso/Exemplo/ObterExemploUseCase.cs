using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comandos.Exemplo.ObterExemplo;
using SME.AE.Aplicacao.Comum.Interfaces;

namespace SME.AE.Aplicacao.CasoDeUso.Exemplo
{
    public class ObterExemploUseCase
    {
        public static async Task<IEnumerable<string>> Executar(IMediator mediator)
        {
            return await mediator.Send(new ObterExemploCommand());
        }
    }
}