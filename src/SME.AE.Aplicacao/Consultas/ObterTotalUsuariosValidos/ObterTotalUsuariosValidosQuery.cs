using MediatR;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterTotalUsuariosValidos
{
    public class ObterTotalUsuariosValidosQuery : IRequest<long>
    {
        public List<string> Cpfs { get; set; }

        public ObterTotalUsuariosValidosQuery(List<string> cpfs)
        {
            Cpfs = cpfs;
        }
    }
}
