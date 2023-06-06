using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Api.Filtros;
using SME.AE.Aplicacao.Comum.Interfaces;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class FrequenciaController : ApiController
    {
        [HttpPost("transferir-frequencia")]
        [AllowAnonymous]
        [ChaveIntegracaoFiltro]
        public async Task<IActionResult> TransferirFrequencia([FromServices] ITransferirFrequenciaSgpCasoDeUso transferirFrequenciaSgpCasoDeUso)
        {
            await transferirFrequenciaSgpCasoDeUso
                .ExecutarAsync();

            return Ok();
        }
    }
}
