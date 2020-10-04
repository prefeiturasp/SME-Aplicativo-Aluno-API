using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class EventoController: ApiController
    {
        [HttpGet("AlunoLogado/{ano}/{mes}/{codigoAluno}")]
        [Authorize]
        public async Task<ActionResult<EventoRespostaDto>> ObterDoAlunoLogado(long codigoAluno, int mes, int ano, [FromServices] IObterEventosAlunoPorMesUseCase obterDoAlunoLogado)
        {
            return Ok(await obterDoAlunoLogado.Executar(User.Identity.Name, codigoAluno, mes, ano));
        }
    }
}
