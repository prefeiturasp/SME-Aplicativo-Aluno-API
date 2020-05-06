using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.CasoDeUso.UsuarioNotificacaoMensagemLida;

namespace SME.AE.Api.Controllers
{

    public class UsuarioNotificacaoController : ApiController
    {
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> MarcarMensagemLida([FromBody] Aplicacao.Comum.Modelos.Entrada.UsuarioNotificacao usuarioMensagem)
        {
            try
            {
                return Ok(await MarcarMensagemLidaUseCase.Executar(Mediator, usuarioMensagem));
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
    }
}
