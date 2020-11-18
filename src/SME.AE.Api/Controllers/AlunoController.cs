using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Frequencia;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
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

        [HttpGet("frequencia/componente-curricular")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterFrequenciaAluno([FromQuery] ObterFrequenciaAlunoPorComponenteCurricularDto frequenciaAlunoDto, [FromServices] IObterFrequenciaAlunoPorComponenteCurricularUseCase obterFrequenciaAlunoPorComponenteCurricularUseCase)
        {
            return Ok(await obterFrequenciaAlunoPorComponenteCurricularUseCase.Executar(frequenciaAlunoDto));
        }

        [HttpGet("frequencia")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterFrequenciaAluno([FromQuery] ObterFrequenciaAlunoDto frequenciaAlunoDto, [FromServices] IObterFrequenciaAlunoUseCase obterFrequenciaGlobalAlunoUseCase)
        {
            return Ok(await obterFrequenciaGlobalAlunoUseCase.Executar(frequenciaAlunoDto));
        }

        [HttpGet("notas")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterNotasAluno([FromQuery] NotaAlunoDto notaAlunoDto,[FromServices] IObterNotasAlunoUseCase obterNotasAlunoUseCase)
        {
            return Ok(await obterNotasAlunoUseCase.Executar(notaAlunoDto));
        }
    }
}
