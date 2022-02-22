using MediatR;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioPorTokenRedefinicao
{
    public class ObterUsuarioPorTokenRedefinicaoQuery : IRequest<Usuario>
    {
        public ObterUsuarioPorTokenRedefinicaoQuery(string token)
        {
            Token = token;
        }

        public string Token { get; set; }
    }
}
