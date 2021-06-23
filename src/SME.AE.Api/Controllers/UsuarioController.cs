using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsuarioController :  ControllerBase
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
            var retorno = await atualizarDadosUsuarioUseCase.Executar(usuarioDto);
            if (retorno.Ok)
                return Ok(retorno);
            else
                return StatusCode(601, retorno);
        }
        [HttpGet("{cpf}")]
        [Authorize]
        public async Task<ObjectResult> ObterDadosUsuario(string cpf, [FromServices] IObterDadosUsuarioPorCpfUseCase obterDadosUsuarioPorCpfUseCase)
        {
            return Ok(await obterDadosUsuarioPorCpfUseCase.Executar(cpf));
        }

    }
}