using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    [Route("api/v1/dashboard")]
    public class DashboardController : ApiController
    {
        [HttpGet("dre/{codigoDre}/ue/{codigoUe}/acesso-incompleto")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterTotalUsuariosComAcessoIncompleto(string codigoDre, string codigoUe, [FromServices] IObterTotalUsuariosComAcessoIncompletoUseCase obterTotalUsuariosComAcessoIncompletoUseCase)
        {
            return Ok(await obterTotalUsuariosComAcessoIncompletoUseCase.Executar(codigoDre, codigoUe));
        }

    }
}