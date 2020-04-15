using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<RespostaApi> Remover([FromBody] long[] ids)
        {
            return await RemoverNotificacaoEmLoteUseCase.Executar(Mediator, ids);
        }

        [AllowAnonymous]
        [ChaveIntegracaoFiltro]
        [HttpDelete("{id}")]
        public async Task<bool> RemoverPorID([FromBody] int id)
        {
            return await RemoveNotificacaoPorIdUseCase.Executar(Mediator, id);
        }


        [HttpGet("{cpf}")]
        [Authorize]
        public async Task<IEnumerable<Notificacao>> ObterDoUsuarioLogado(string cpf)
        {
            return await ObterDoUsuarioLogadoUseCase.Executar(Mediator, User.Identity.Name);
        }
    }
}
