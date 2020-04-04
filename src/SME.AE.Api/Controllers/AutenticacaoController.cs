using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SME.AE.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacaoController : ApiController
    {
        [HttpPost]
        public async Task<ActionResult<RespostaAutenticar>> Autenticar([FromBody] Usuario usuario)
        {
            return Ok(await CriarUsuarioUseCase.Executar(Mediator, usuario));
        }
    }
}
