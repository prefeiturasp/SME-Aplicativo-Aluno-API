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
        [HttpPost("raa")]
        public async Task<IActionResult> SolicitarRelatorioRaa([FromBody] SolicitarRelatorioRaaDto filtro,[FromServices] ISolicitarRelatorioRaaUseCase useCase)
        {
            return Ok(await useCase.Executar(filtro));
        }
        [HttpPost("boletim")]
        public async Task<IActionResult> SolicitarBoletim([FromBody] SolicitarBoletimAlunoDto filtro, [FromServices] ISolicitarBoletimAlunoUseCase solicitarBoletimAluno)
        {
            return Ok(await solicitarBoletimAluno.Executar(filtro));
        }
    }
}
