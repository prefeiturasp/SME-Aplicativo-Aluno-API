using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Recomendacao;
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
        public async Task<IActionResult> ObterComponentesCurriculares(string ueCodigo, string turmaCodigo, string alunoCodigo, [FromQuery] int[] bimestres, [FromServices] IObterComponentesCurricularesIdsUseCase useCase)
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

        [HttpGet("recomendacao-aluno")]
        public async Task<IActionResult> ObterRecomendacoesAluno([FromQuery] FiltroRecomendacaoAlunoDto filtro, [FromServices] IObterRecomendacaoAlunoUseCase useCase)
        {
            return Ok(await useCase.Executar(filtro));
        }
    }
}
