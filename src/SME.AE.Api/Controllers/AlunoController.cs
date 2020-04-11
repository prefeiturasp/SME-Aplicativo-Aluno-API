using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SME.AE.Api.Controllers
{
 
    [ApiController]
    public class AlunoController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<string>> ObterDadosAlunos([FromQuery] string cpf)
        {
            // TODO Pegar o Token
            return Ok(await AutenticarUsuarioUseCase.Executar(Mediator, cpf, senha));
        }
    }
}
