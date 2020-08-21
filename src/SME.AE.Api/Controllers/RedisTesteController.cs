using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;

namespace SME.AE.Api.Controllers
{    
    [AllowAnonymous]
    [ApiController]
    [Route("api/v1/redis")]
    public class RedisTesteController : ApiController
    {
        [HttpPost]
        public async Task<ActionResult> ObterDadosAlunos([FromQuery] string cpf, [FromServices] ICacheUseCase cacheUseCase)
        {
            return Ok(await cacheUseCase.Executar(cpf));
        }
    }
}
