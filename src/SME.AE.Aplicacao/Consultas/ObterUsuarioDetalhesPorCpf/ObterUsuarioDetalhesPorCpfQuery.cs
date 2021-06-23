using MediatR;

namespace SME.AE.Aplicacao
{
    public class ObterUsuarioDetalhesPorCpfQuery : IRequest<UsuarioDadosDetalhesDto>
    {
        public ObterUsuarioDetalhesPorCpfQuery(string cpf)
        {
            Cpf = cpf;
        }

        public string Cpf { get; set; }
    }
}
