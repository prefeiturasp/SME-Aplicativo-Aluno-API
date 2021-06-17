using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class UsuarioController : ApiController
    {
        [HttpGet("dre/{codigoDre}/ue/{codigoUe}/cpf/{cpf}")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterUsuariosPorCpf(string codigoDre, long codigoUe, string cpf, [FromServices] IObterUsuarioUseCase obterUsuarioUseCase)
        {
            return Ok(await obterUsuarioUseCase.Executar(codigoDre, codigoUe, cpf));
        }

        [HttpPut]
        [Authorize]
        public async Task<ObjectResult> AtualizarDadosUsuario([FromBody] AtualizarDadosUsuarioDto usuarioDto, [FromServices] IAtualizarDadosUsuarioUseCase atualizarDadosUsuarioUseCase)
        {
            return Ok(await atualizarDadosUsuarioUseCase.Executar(usuarioDto));
        }

    }
}