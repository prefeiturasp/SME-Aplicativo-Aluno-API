using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas
{
    public class ObterComunicadosAnoAtualQuery : IRequest<IEnumerable<ComunicadoSgpDto>>
    {
        public ObterComunicadosAnoAtualQuery()
        {}
    }
}
