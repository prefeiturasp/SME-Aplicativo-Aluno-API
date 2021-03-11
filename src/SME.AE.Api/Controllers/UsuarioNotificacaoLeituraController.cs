using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Api.Filtros;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    [Route("api/v1/UsuarioNotificacaoLeitura")]
    public class UsuarioNotificacaoLeituraController : ApiController
    {
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> MarcarMensagemLida([FromBody] UsuarioNotificacaoDto usuarioMensagem, [FromServices] IMarcarMensagemLidaUseCase marcarMensagemLidaUseCase)
        {
            return Ok(await marcarMensagemLidaUseCase.Executar(Mediator, usuarioMensagem, User.Identity.Name));
        }

        [HttpGet("status-leitura")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterStatusDeLeituraNotificacao([FromQuery] List<long> notificacaoId, long codigoAluno, [FromServices] IObterStatusDeLeituraNotificacaoUseCase useCase)
        {
            return Ok(await useCase.Executar(notificacaoId, codigoAluno));
        }
    }
}
