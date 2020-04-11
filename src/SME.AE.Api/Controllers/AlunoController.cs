﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.CasoDeUso.Aluno;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Api.Controllers
{
 
    [ApiController]
    public class AlunoController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IEnumerable<Aluno>> ObterDadosAlunos([FromQuery] string cpf)
        {
            // TODO Pegar o Token
            return await DadosDoAlunoUseCase.Executar(Mediator, cpf);
        }
    }
}
