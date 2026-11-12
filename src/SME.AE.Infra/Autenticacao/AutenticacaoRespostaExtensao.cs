using SME.AE.Aplicacao.Comum.Modelos;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace SME.AE.Infra.Autenticacao
{
    [ExcludeFromCodeCoverage]
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