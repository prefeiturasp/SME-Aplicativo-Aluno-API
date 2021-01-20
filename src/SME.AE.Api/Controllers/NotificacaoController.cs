using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Api.Filtros;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class NotificacaoController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        //[ChaveIntegracaoFiltro]
        public async Task<ObjectResult> Criar([FromBody] NotificacaoSgpDto notificacao, [FromServices] ICriarNotificacaoUseCase criarNotificacaoUseCase)
        {
            return Ok(await criarNotificacaoUseCase.Executar(notificacao));
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        //[ChaveIntegracaoFiltro]
        public async Task<ObjectResult> Atualizar([FromBody] NotificacaoSgpDto notificacao, [FromServices] IAtualizarNotificacaoUseCase atualizarNotificacaoUseCase)
        {
            return Ok(await atualizarNotificacaoUseCase.Executar(notificacao));
        }

        [HttpDelete]
        [AllowAnonymous]
        [ChaveIntegracaoFiltro]
        public async Task<ActionResult> Remover([FromBody] long[] ids, [FromServices] IRemoverNotificacaoEmLoteUseCase removerNotificacaoEmLoteUseCase)
        {
            return Ok(await removerNotificacaoEmLoteUseCase.Executar(ids));
        }

        [AllowAnonymous]
        [ChaveIntegracaoFiltro]
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoverPorID(int id, [FromServices] IRemoveNotificacaoPorIdUseCase removeNotificacaoPorIdUseCase)
        {
            return Ok(await removeNotificacaoPorIdUseCase.Executar(id));
        }

        [HttpGet("{codigoAluno}")]
        [Authorize]
        public async Task<ObjectResult> ObterDoUsuarioLogado(long codigoAluno, [FromServices] IObterNotificacaoDoUsuarioLogadoUseCase obterDoUsuarioLogadoUseCase)
        {
            return Ok(await obterDoUsuarioLogadoUseCase.Executar(User.Identity.Name, codigoAluno));
        }
    }
}
