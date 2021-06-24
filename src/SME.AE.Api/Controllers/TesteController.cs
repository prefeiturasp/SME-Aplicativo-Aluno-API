using Microsoft.AspNetCore.Mvc;
using SME.AE.Api.Filtros;
using System;

namespace SME.AE.Api.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]

    public class TesteController : ControllerBase
    {
        public TesteController()
        {

        }

        [HttpGet("obter-data-hora")]
        [ChaveIntegracaoFiltro]
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

    }
}
