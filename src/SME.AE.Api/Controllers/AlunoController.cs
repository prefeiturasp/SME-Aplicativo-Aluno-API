using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.CasoDeUso.Aluno;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Api.Controllers
{
 
    [ApiController]
    public class AlunoController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ObterDadosAlunos([FromQuery] string cpf)
        {
            try
            {
                return Ok(await DadosDoAlunoUseCase.Executar(Mediator, cpf));
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
    }
}
