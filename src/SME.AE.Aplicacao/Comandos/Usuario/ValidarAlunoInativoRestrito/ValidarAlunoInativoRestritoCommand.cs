using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;

namespace SME.AE.Aplicacao.Comandos.Usuario.ValidarAlunoInativoRestrito
{
    public class ValidarAlunoInativoRestritoCommand : IRequest<Unit>
    {
        public ValidarAlunoInativoRestritoCommand(RetornoUsuarioCoreSSO usuarioCoreSSO)
        {
            UsuarioCoreSSO = usuarioCoreSSO;
        }

        public RetornoUsuarioCoreSSO UsuarioCoreSSO { get; set; }

    }
}
