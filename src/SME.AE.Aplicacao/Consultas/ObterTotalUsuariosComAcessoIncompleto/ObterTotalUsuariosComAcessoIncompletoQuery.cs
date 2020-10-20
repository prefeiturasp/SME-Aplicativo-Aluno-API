using MediatR;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterTotalUsuariosComAcessoIncompleto
{
    public class ObterTotalUsuariosComAcessoIncompletoQuery : IRequest<long>
    {
        public List<string> Cpfs { get; set; }

        public ObterTotalUsuariosComAcessoIncompletoQuery(List<string> cpfs)
        {
            Cpfs = cpfs;
        }
    }
}
