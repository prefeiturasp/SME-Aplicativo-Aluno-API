using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    [Route("api/v1/dashboard")]
    public class DashboardController : ApiController
    {
        [HttpGet("adesao")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterTotaisAdesao([FromQuery] string codigoDre, [FromQuery] string codigoUe, [FromServices] IObterTotaisAdesaoUseCase obterTotaisAdesaoUseCase)
        {
            return Ok(await obterTotaisAdesaoUseCase.Executar(codigoDre, codigoUe));
        }

        [HttpGet("adesao/agrupados")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterTotaisAdesaoAgrupadosPorDre([FromServices] IObterTotaisAdesaoAgrupadosPorDreUseCase obterTotaisAdesaoAgrupadosPorDreUseCase)
        {
            return Ok(await obterTotaisAdesaoAgrupadosPorDreUseCase.Executar());
        }

        [HttpGet("adesao/usuarios/incompletos")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterTotalUsuariosComAcessoIncompleto([FromQuery] string codigoDre, [FromQuery] string codigoUe, [FromServices] IObterTotalUsuariosComAcessoIncompletoUseCase obterTotalUsuariosComAcessoIncompletoUseCase)
        {
            return Ok(await obterTotalUsuariosComAcessoIncompletoUseCase.Executar(codigoDre, codigoUe));
        }

        [HttpGet("adesao/usuarios/validos")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterTotalUsuariosValidos([FromQuery] string codigoDre, [FromQuery] string codigoUe, [FromServices] IObterTotalUsuariosValidosUseCase obterTotalUsuariosValidosUseCase)
        {
            return Ok(await obterTotalUsuariosValidosUseCase.Executar(codigoDre, codigoUe));
        }

        [HttpGet("leitura")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterDadosDeLeituraComunicados([FromQuery] string codigoDre, [FromQuery] string codigoUe, [FromServices] IObterDadosDeLeituraComunicadosUseCase obterDadosDeLeituraComunicados)
        {
            return Ok(await obterDadosDeLeituraComunicados.Executar(codigoDre, codigoUe));
        }

    }
}