using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.Dre;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas
{
    public class ObterDresQuery : IRequest<IEnumerable<DreResposta>>
    {
        public ObterDresQuery()
        {}
    }
}
