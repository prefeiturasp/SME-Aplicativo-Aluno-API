using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.CasoDeUso.Usuario.Excecoes;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Api.Controllers
{
    public class UsuarioController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Usuario>> ObterPorCpf([FromQuery] string cpf)
        {
            Usuario usuario;

            try
            {
                usuario = await ObterUsuarioPorCpfUseCase.Executar(Mediator, cpf);
            }
            catch (UsuarioNaoEncontradoException ex)
            {
                return NotFound(ex.Message);
            }
            
            return Ok(usuario);
        }
    }
}
