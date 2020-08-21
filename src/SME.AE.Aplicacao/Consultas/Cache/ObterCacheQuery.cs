using MediatR;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterCacheQuery : IRequest<string>
    {
        public ObterCacheQuery(string cpf)
        {
            Cpf = cpf;
        }

        public string Cpf { get; set; }
    }
}
