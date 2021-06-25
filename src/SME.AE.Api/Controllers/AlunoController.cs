using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum.Interfaces;
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
        public async Task<ActionResult> ObterDadosAlunos([FromQuery] string cpf, [FromServices] IDadosDoAlunoUseCase dadosDoAlunoUseCase)
        {
            return Ok(await dadosDoAlunoUseCase.Executar(cpf));
        }

        [HttpGet("frequencia/componente-curricular")]
        public async Task<ObjectResult> ObterFrequenciaAluno([FromQuery] ObterFrequenciaAlunoPorComponenteCurricularDto frequenciaAlunoDto, [FromServices] IObterFrequenciaAlunoPorComponenteCurricularUseCase obterFrequenciaAlunoPorComponenteCurricularUseCase)
        {
            return Ok(await obterFrequenciaAlunoPorComponenteCurricularUseCase.Executar(frequenciaAlunoDto));
        }

        [HttpGet("frequencia")]
        public async Task<ObjectResult> ObterFrequenciaAluno([FromQuery] ObterFrequenciaAlunoDto frequenciaAlunoDto, [FromServices] IObterFrequenciaAlunoUseCase obterFrequenciaGlobalAlunoUseCase)
        {
            return Ok(await obterFrequenciaGlobalAlunoUseCase.Executar(frequenciaAlunoDto));
        }

        [HttpGet("notas")]
        public async Task<ObjectResult> ObterNotasAluno([FromQuery] NotaAlunoDto notaAlunoDto,[FromServices] IObterNotasAlunoUseCase obterNotasAlunoUseCase)
        {
            return Ok(await obterNotasAlunoUseCase.Executar(notaAlunoDto));
        }

        [HttpGet("frequencia-global")]
        public async Task<ObjectResult> ObterFrequenciaGlobalAluno([FromQuery] FiltroFrequenciaGlobalAlunoDto filtro, [FromServices] IObterFrequenciaGlobalAlunoUseCase useCase)
        {
            return Ok(await useCase.Executar(filtro));
        }

        [HttpGet("ues/{ueId}/turmas/{turmaId}/alunos/{alunoCodigo}/notas-conceitos")]
        public async Task<ObjectResult> ObterNotasPorBimestresUeAlunoTurma(long ueId, long turmaId, string alunoCodigo, [FromQuery] int[] bimestres, [FromServices] IObterNotasPorBimestresUeAlunoTurmaUseCase useCase)
        {
            return Ok(await useCase.Executar(new NotaConceitoPorBimestresAlunoTurmaDto(ueId, turmaId, alunoCodigo, bimestres)));
        }

        [HttpGet("boletins/liberacoes/bimestres/{turmaCodigo}")]
        public async Task<ObjectResult> ObterBimestresLiberacaoDeBoletimAlunoTurma(string turmaCodigo, [FromServices] IObterBimestresLiberacaoBoletimAlunoUseCase useCase)
        {
            return Ok(await useCase.Executar(turmaCodigo));
        }
    }
}
