using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum;
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
        [HttpGet]
        public async Task<ObjectResult> ObterDadosAlunos([FromQuery] string cpf, [FromServices] IDadosDoAlunoUseCase dadosDoAlunoUseCase)
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

        [HttpGet("ues/{ueCodigo}/turmas/{turmaCodigo}/alunos/{alunoCodigo}/notas-conceitos")]
        public async Task<ObjectResult> ObterNotasPorBimestresUeAlunoTurma(string ueCodigo, string turmaCodigo, string alunoCodigo, [FromQuery] int[] bimestres, [FromServices] IObterNotasPorBimestresUeAlunoTurmaUseCase useCase)
        {
            return Ok(await useCase.Executar(new AlunoBimestresTurmaDto(ueCodigo, turmaCodigo, alunoCodigo, bimestres)));
        }

        [HttpGet("ues/{ueCodigo}/turmas/{turmaCodigo}/alunos/{alunoCodigo}/componentes-curriculares")]
        public async Task<ObjectResult> ObterComponentesCurriculares(string ueCodigo, string turmaCodigo, string alunoCodigo, [FromQuery] int[] bimestres, [FromServices] IObterComponentesCurricularesIdsUseCase useCase)
        {
            return Ok(await useCase.Executar(new AlunoBimestresTurmaDto(ueCodigo, turmaCodigo, alunoCodigo, bimestres)));
        }

        [HttpGet("turmas/{turmaCodigo}/boletins/liberacoes/bimestres")]
        public async Task<IActionResult> ObterBimestresLiberacaoDeBoletimAlunoTurma(string turmaCodigo, [FromServices] IObterBimestresLiberacaoBoletimAlunoUseCase useCase)
        {
            return Ok(await useCase.Executar(turmaCodigo));
        }

        [HttpGet("frequencia/turmas/{turmaCodigo}/alunos/{alunoCodigo}/componentes-curriculares/{componenteCurricularId}")]
        public async Task<IActionResult> ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricular(string turmaCodigo, string alunoCodigo, string componenteCurricularId, [FromQuery] int[] bimestres, [FromServices] IObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularUseCase useCase)
        {
            return Ok(await useCase.Executar(new FrequenciaPorBimestresAlunoTurmaComponenteCurricularDto(turmaCodigo, alunoCodigo, bimestres, componenteCurricularId)));
        }
    }
}
