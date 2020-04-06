﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.CasoDeUso.Exemplo;

namespace SME.AE.Api.Controllers
{
    public class ExemploController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> ObterTodos()
        {
            return Ok(await ObterExemploUseCase.Executar(Mediator));
        }
    }
}
