using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;

namespace SME.AE.Aplicacao.Comandos.CoreSSO.Usuario
{
    public class CriarUsuarioCoreSSOCommand : IRequest<RetornoUsuarioCoreSSO>
    {
        public CriarUsuarioCoreSSOCommand()
        {

        }

        public CriarUsuarioCoreSSOCommand(UsuarioCoreSSO usuario)
        {
            Usuario = usuario;
        }

        public UsuarioCoreSSO Usuario { get; set; }
    }
}
