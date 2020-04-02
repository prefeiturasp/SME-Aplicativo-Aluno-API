using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;

namespace SME.AE.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AutenticacaoController : ApiController
    {
        [HttpPost]
        public async Task<ActionResult<RespostaAutenticar>> Autenticar([FromBody] Usuario usuario)
        {
            return Ok(await CriarUsuarioUseCase.Executar(Mediator));
        }
    }
}