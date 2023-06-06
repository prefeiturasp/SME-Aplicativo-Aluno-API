using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Api.Filtros;
using SME.AE.Aplicacao.Comum.Interfaces;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class NotaController : ApiController
    {
        [HttpPost("transferir-nota")]
        [AllowAnonymous]
        [ChaveIntegracaoFiltro]
        public async Task<IActionResult> TransferirNota([FromServices] ITransferirNotaSgpCasoDeUso transferirNotaSgpCasoDeUso)
        {
            await transferirNotaSgpCasoDeUso
                .ExecutarAsync();

            return Ok();
        }
    }
}
