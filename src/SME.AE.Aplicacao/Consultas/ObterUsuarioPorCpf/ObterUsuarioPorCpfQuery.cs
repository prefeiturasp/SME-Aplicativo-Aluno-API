using MediatR;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterUsuarioPorCpfQuery : IRequest<Usuario>
    {
        public ObterUsuarioPorCpfQuery(string cpf)
        {
            Cpf = cpf;
        }

        public string Cpf { get; set; }
    }
}
