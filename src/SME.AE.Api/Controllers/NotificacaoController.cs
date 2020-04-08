using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Api.Filtros;
using SME.AE.Aplicacao.CasoDeUso.Notificacao;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Api.Controllers
{
    public class NotificacaoController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        [ChaveIntegracaoFiltro]
        public async Task<Notificacao> Criar([FromBody] SME.AE.Dominio.Entidades.Notificacao notificacao)
        {
            return await CriarNotificacaoUseCase.Executar(Mediator, notificacao);
        }
        
        [HttpPut]
        [AllowAnonymous]
        [ChaveIntegracaoFiltro]
        public async Task<Notificacao> Atualizar([FromBody] SME.AE.Dominio.Entidades.Notificacao notificacao)
        {
            return await AtualizarNotificacaoUseCase.Executar(Mediator, notificacao);
        }
        
        [HttpDelete]
        [AllowAnonymous]
        [ChaveIntegracaoFiltro]
        public async Task<bool> Remover([FromBody] SME.AE.Dominio.Entidades.Notificacao notificacao)
        {
            return await RemoverNotificacaoUseCase.Executar(Mediator, notificacao);
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Notificacao>> ObterPorGrupo([FromQuery] string grupo)
        {
            return await ObterNotificacaoPorGrupoUseCase.Executar(Mediator, grupo);
        }
    }
}
