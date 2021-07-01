using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterParametrosSistemaPorChavesQuery : IRequest<IEnumerable<ParametroEscolaAqui>>
    {
        public IEnumerable<string> Chaves { get; set; }

        public ObterParametrosSistemaPorChavesQuery(IEnumerable<string> chaves)
        {
            Chaves = chaves;
        }
    }
}
