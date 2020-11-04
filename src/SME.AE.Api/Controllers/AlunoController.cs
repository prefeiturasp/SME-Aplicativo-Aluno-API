using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    [ApiController]
    public class AlunoController : ApiController
    {
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> ObterDadosAlunos([FromQuery] string cpf, [FromServices] IDadosDoAlunoUseCase dadosDoAlunoUseCase)
        {
            return Ok(await dadosDoAlunoUseCase.Executar(cpf));
        }

        [HttpGet("frequencia")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterFrequenciaAluno([FromQuery] int anoLetivo, [FromQuery] string codigoUe, [FromQuery] long codigoTurma, [FromQuery] string codigoAluno, [FromServices] IObterFrequenciaAlunoUseCase obterFrequenciaAlunoUseCase)
        {
            return Ok(await obterFrequenciaAlunoUseCase.Executar(anoLetivo, codigoUe, codigoTurma, codigoAluno));
        }

        [HttpGet("notas")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterNotasAluno([FromQuery] int anoLetivo, [FromQuery] string codigoUe, [FromQuery] string codigoTurma, [FromQuery] string codigoAluno, [FromServices] IObterNotasAlunoUseCase obterNotasAlunoUseCase)
        {
            return Ok(await obterNotasAlunoUseCase.Executar(anoLetivo, codigoUe, codigoTurma, codigoAluno));
        }
    }
}
