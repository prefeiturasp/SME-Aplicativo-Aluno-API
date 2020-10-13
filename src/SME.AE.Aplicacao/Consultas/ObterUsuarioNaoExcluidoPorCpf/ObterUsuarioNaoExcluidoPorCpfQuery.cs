using MediatR;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterUsuarioNaoExcluidoPorCpfQuery : IRequest<Usuario>
    {
        public ObterUsuarioNaoExcluidoPorCpfQuery(string cpf)
        {
            Cpf = cpf;
        }

        public string Cpf { get; set; }
    }
}
