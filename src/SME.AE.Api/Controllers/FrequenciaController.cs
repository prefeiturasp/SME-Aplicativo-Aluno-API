using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Api.Filtros;
using SME.AE.Aplicacao.Comum.Interfaces;
using System;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class FrequenciaController : ApiController
    {
        [HttpPost("transferir-frequencia")]
        [AllowAnonymous]
        [ChaveIntegracaoFiltro]
        public async Task<IActionResult> TransferirFrequencia([FromQuery] int anoLetivo, [FromQuery] long ueId, [FromServices] ITransferirFrequenciaSgpCasoDeUso transferirFrequenciaSgpCasoDeUso)
        {
            if (anoLetivo == 0) throw new ArgumentNullException(nameof(anoLetivo));

            if (ueId == 0) throw new ArgumentNullException(nameof(ueId));

            await transferirFrequenciaSgpCasoDeUso
                .ExecutarAsync(anoLetivo, ueId);

            return Ok();
        }
    }
}
