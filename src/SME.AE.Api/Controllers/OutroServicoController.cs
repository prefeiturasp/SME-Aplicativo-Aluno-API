using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    [Route("api/v1/outroservico")]
    [ApiController]
    public class OutroServicoController : ApiController
    {
        [HttpGet("links/destaque")]
        public async Task<ObjectResult> ObterLinksDestaque([FromServices] IOutroServicoUseCase useCase)
        {
            return Ok(await useCase.LinksPrioritarios());
        }

        [HttpGet("links/lista")]
        public async Task<ObjectResult> ObterTodosLinks([FromServices] IOutroServicoUseCase useCase)
        {
            return Ok(await useCase.Links());
        }
    }
}
