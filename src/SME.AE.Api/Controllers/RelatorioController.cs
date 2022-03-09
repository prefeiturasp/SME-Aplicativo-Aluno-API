using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao;
using System;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    [ApiController]
    public class RelatorioController : ApiController
    {
        [HttpPost("existe")]
        public async Task<IActionResult> VerificarSeRelatorioExiste([FromBody] Guid codigoRelatorio, [FromServices] IRelatorioImpressaoUseCase useCase)
        {
            return Ok(await useCase.Executar(codigoRelatorio));
        }
    }
}
