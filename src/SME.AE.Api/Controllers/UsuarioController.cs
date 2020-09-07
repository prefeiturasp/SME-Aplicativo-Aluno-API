using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class UsuarioController : ApiController
    {
        [HttpGet("{cpf}")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterUsuariosPorCpf(string cpf, [FromServices] IObterUsuarioUseCase obterUsuarioUseCase)
        {
            return Ok(await obterUsuarioUseCase.Executar(cpf));
        }

    }
}