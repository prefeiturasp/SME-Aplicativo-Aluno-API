using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;

namespace SME.AE.Aplicacao.Comandos.CoreSSO.Usuario
{
    public class CriarUsuarioCoreSSOCommand : IRequest<RetornoUsuarioCoreSSO>
    {
        public UsuarioCoreSSO Usuario { get; set; }
    }
}
