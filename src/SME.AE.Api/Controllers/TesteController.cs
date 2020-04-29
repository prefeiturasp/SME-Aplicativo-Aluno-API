using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.CasoDeUso.Aluno;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Api.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class TesteController : ControllerBase
    {
        private readonly ITesteRepositorio _testeRepositoriio;
        public TesteController(ITesteRepositorio testeRepositoriio)
        {
            _testeRepositoriio = testeRepositoriio;
        }

        [HttpGet("obter-data-hora")]
        [AllowAnonymous]
        public ActionResult ObterDataHoraAtual()
        {
            try
            {
                return Ok(DateTime.Now);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpGet("obter-data-hora-servidor")]
        [AllowAnonymous]
        public async Task<ActionResult> ObterDataHoraAtualBanco()
        {
            try
            {
                return Ok(await _testeRepositoriio.ObterDataHoraBanco());
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
    }
}
