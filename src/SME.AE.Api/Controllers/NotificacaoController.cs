using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using SME.AE.Api.Filtros;
using SME.AE.Aplicacao.CasoDeUso.Notificacao;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Api.Controllers
{
    public class NotificacaoController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        [ChaveIntegracaoFiltro]
        public async Task<ObjectResult> Criar([FromBody] SME.AE.Dominio.Entidades.Notificacao notificacao)
        {
            try
            {
                return Ok(await CriarNotificacaoUseCase.Executar(Mediator, notificacao));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(400, ex.Message);
            }
        }
        
        [HttpPut("{id}")]
        [AllowAnonymous]
        [ChaveIntegracaoFiltro]
        public async Task<ObjectResult> Atualizar([FromBody] SME.AE.Dominio.Entidades.Notificacao notificacao)
        {
            try
            {
                return Ok(await AtualizarNotificacaoUseCase.Executar(Mediator, notificacao));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(400, ex.Message);
            }
        }
        
        [HttpDelete]
        [AllowAnonymous]
      //  [ChaveIntegracaoFiltro]
        public async Task<ActionResult> Remover([FromBody] long[] ids)
        {
            try
            {
                return Ok(await RemoverNotificacaoEmLoteUseCase.Executar(Mediator, ids));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(400, ex.Message);
            }
        }

        [AllowAnonymous]
        [ChaveIntegracaoFiltro]
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoverPorID([FromBody] int id)
        {
            try
            {
                return Ok(await RemoveNotificacaoPorIdUseCase.Executar(Mediator, id));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(400, ex.Message);
            }
        }
        
        [HttpGet]
        [Authorize]
        public async Task<ObjectResult> ObterDoUsuarioLogado()
        {
            try
            {
                return Ok(await ObterDoUsuarioLogadoUseCase.Executar(Mediator, User.Identity.Name));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(400, ex.Message);
            }
        }
    }
}
