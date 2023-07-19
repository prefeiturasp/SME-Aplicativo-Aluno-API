using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Api.Filtros;
using SME.AE.Aplicacao.Comum.Interfaces;
using System;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class NotaController : ApiController
    {
        [HttpPost("transferir-nota")]
        [AllowAnonymous]
        [ChaveIntegracaoFiltro]
        public async Task<IActionResult> TransferirNota([FromQuery] int anoLetivo, [FromQuery] long ueId, [FromServices] ITransferirNotaSgpCasoDeUso transferirNotaSgpCasoDeUso)
        {
            if (anoLetivo == 0) throw new ArgumentNullException(nameof(anoLetivo));

            if (ueId == 0) throw new ArgumentNullException(nameof(ueId));

            await transferirNotaSgpCasoDeUso
                .ExecutarPorAnoUeAsync(anoLetivo, ueId);

            return Ok();
        }
    }
}
