using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;

namespace SME.AE.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AutenticacaoController : ApiController
    {
        [HttpPost]
        public async Task<ActionResult<RespostaApi>> Autenticar([FromBody] Usuario usuario)
        {
            return Ok(await AutenticarUsuarioUseCase.Executar(Mediator, usuario));
        }
    }
}
