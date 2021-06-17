using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.CasoDeUso.Aluno;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Api.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
   // [Authorize]
    public class TesteController : ControllerBase
    {
        private readonly ITesteRepositorio _testeRepositoriio;
        private readonly IMediator mediator;
        public TesteController(ITesteRepositorio testeRepositoriio, IMediator mediator)
        {
            _testeRepositoriio = testeRepositoriio;
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("obter-data-hora")]
      //  [Authorize]
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
      //  [Authorize]
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

        //TODO : Retirar antes de subir para a história
        [HttpGet("palavras-proibidas")]
       // [Authorize]
        public async Task<ActionResult> VaridarPalavrasProibidas()
        {
            try
            {
                // return Ok(await _testeRepositoriio.ObterDataHoraBanco());
                return Ok(await mediator.Send(new VerificaPalavraProibidaPodePersistirCommand(" Teste de verificação de PALAVRAS proibidas")));
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
    }
}
