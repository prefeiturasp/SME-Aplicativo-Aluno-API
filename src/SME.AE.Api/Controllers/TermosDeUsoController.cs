using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class TermosDeUsoController : ApiController
    {
        [HttpGet()]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterUsuariosPorCpf([FromServices] IObterTermosDeUsoUseCase obterTermosDeUsoUseCase)
        {
            return Ok(await obterTermosDeUsoUseCase.Executar());
        }
    }
}