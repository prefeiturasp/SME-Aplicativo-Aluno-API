using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    [ApiController]
    public class UnidadeEscolarController : ApiController
    {
        [HttpGet("{codigoUe}")]
        [Authorize]
        public async Task<ObjectResult> ObterDadosUnidadeEscolarPorCodigo(string codigoUe, [FromServices] IObterDadosUnidadeEscolarUseCase obterDadosUnidadeEscolarUseCase)
        {
            return Ok(await obterDadosUnidadeEscolarUseCase.Executar(codigoUe));
        }
    }
}
