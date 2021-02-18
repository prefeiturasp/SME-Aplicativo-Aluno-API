using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class UsuarioNotificacaoLeituraController : ApiController
    {
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> MarcarMensagemLida([FromBody] UsuarioNotificacaoDto usuarioMensagem, [FromServices] IMarcarMensagemLidaUseCase marcarMensagemLidaUseCase)
        {
            return Ok(await marcarMensagemLidaUseCase.Executar(Mediator, usuarioMensagem, User.Identity.Name));
        }
    }
}
