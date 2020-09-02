using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using SME.AE.Api.Filtros;
using SME.AE.Aplicacao.CasoDeUso.Notificacao;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class MensagensController : ApiController
    {
        [HttpGet("{codigoAluno}/desde/{dataUltimaConsulta}")]
        [Authorize]
        public async Task<ObjectResult> MensagensUsuarioLogadoAlunoDesdeData(long codigoAluno, DateTime dataUltimaConsulta, [FromServices]IMensagensUsuarioLogadoAlunoUseCase mensagensUsuarioLogadoAluno)
        {

            return Ok(await mensagensUsuarioLogadoAluno.Executar(User.Identity.Name, codigoAluno, dataUltimaConsulta));
        }
    }
}
