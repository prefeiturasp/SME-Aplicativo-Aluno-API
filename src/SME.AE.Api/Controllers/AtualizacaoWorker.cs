using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.UltimaAtualizacaoWorker;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    [Route("api/v1/worker")]
    public class AtualizacaoWorkerController : ApiController
    {
        [HttpGet("ultimaAtualizacao")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterUltimaAtualizacaoPorProcesso([FromQuery] string nomeProcesso, [FromServices] IObterUltimaAtualizacaoPorProcessoUseCase obterUltimaAtualizacaoPorProcessoUseCase)
        {
            return Ok(await obterUltimaAtualizacaoPorProcessoUseCase.Executar(nomeProcesso));
        }
    }
}