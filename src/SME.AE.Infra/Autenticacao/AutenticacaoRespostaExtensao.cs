using SME.AE.Aplicacao.Comum.Modelos;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace SME.AE.Infra.Autenticacao
{
    public static class AutenticacaoRespostaExtensao
    {
        public static RespostaApi ParaRespostaApi(this IdentityResult resultado)
        {
            return resultado.Succeeded
                ? RespostaApi.Sucesso()
                : RespostaApi.Falha(resultado.Errors.Select(e => e.Description));
        }
    }
}