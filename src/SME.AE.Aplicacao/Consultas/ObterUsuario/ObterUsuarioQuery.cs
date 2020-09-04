using MediatR;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterUsuarioQuery : IRequest<Usuario>
    {
        public ObterUsuarioQuery()
        {

        }

        public ObterUsuarioQuery(string cpf)
        {
            Cpf = cpf;
        }

        public ObterUsuarioQuery(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
        public string Cpf { get; set; }
    }
}
