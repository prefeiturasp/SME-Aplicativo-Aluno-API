using MediatR;
using System.Collections.Generic;

namespace SME.AE.Aplicacao
{
    public class ObterTurmasModalidadesPorCodigosQuery : IRequest<IEnumerable<TurmaModalidadeCodigoDto>>
    {
        public ObterTurmasModalidadesPorCodigosQuery(string[] turmaCodigo)
        {
            TurmaCodigo = turmaCodigo;
        }

        public string[] TurmaCodigo { get; set; }
    }
}
