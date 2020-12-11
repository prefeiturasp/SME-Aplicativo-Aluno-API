﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    [Route("api/v1/dashboard")]
    public class DashboardController : ApiController
    {
        [HttpGet("adesao")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterTotaisAdesao([FromQuery] string codigoDre, [FromQuery] string codigoUe, [FromServices] IObterTotaisAdesaoUseCase obterTotaisAdesaoUseCase)
        {
            return Ok(await obterTotaisAdesaoUseCase.Executar(codigoDre, codigoUe));
        }

        [HttpGet("adesao/agrupados")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterTotaisAdesaoAgrupadosPorDre([FromServices] IObterTotaisAdesaoAgrupadosPorDreUseCase obterTotaisAdesaoAgrupadosPorDreUseCase)
        {
            return Ok(await obterTotaisAdesaoAgrupadosPorDreUseCase.Executar());
        }

        [HttpGet("adesao/usuarios/incompletos")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterTotalUsuariosComAcessoIncompleto([FromQuery] string codigoDre, [FromQuery] string codigoUe, [FromServices] IObterTotalUsuariosComAcessoIncompletoUseCase obterTotalUsuariosComAcessoIncompletoUseCase)
        {
            return Ok(await obterTotalUsuariosComAcessoIncompletoUseCase.Executar(codigoDre, codigoUe));
        }

        [HttpGet("adesao/usuarios/validos")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterTotalUsuariosValidos([FromQuery] string codigoDre, [FromQuery] string codigoUe, [FromServices] IObterTotalUsuariosValidosUseCase obterTotalUsuariosValidosUseCase)
        {
            return Ok(await obterTotalUsuariosValidosUseCase.Executar(codigoDre, codigoUe));
        }

        [HttpGet("leitura")]
        [AllowAnonymous]
        //[ChaveIntegracaoFiltro]]
        public async Task<ObjectResult> ObterDadosDeLeituraComunicados([FromQuery] string codigoDre, [FromQuery] string codigoUe, [FromQuery] long notificacaoId, ModoVisualizacao modoVisualizacao, [FromServices] IObterDadosDeLeituraComunicadosUseCase obterDadosDeLeituraComunicados)
        {
            return Ok(await obterDadosDeLeituraComunicados.Executar(codigoDre, codigoUe, notificacaoId, modoVisualizacao));
        }

        [HttpGet("leitura/agrupados")]
        [AllowAnonymous]
        //[ChaveIntegracaoFiltro]]
        public async Task<ObjectResult> ObterDadosDeLeituraComunicadosAgrupadosPorDre([FromQuery] long notificacaoId, ModoVisualizacao modoVisualizacao, [FromServices] IObterDadosDeLeituraComunicadosAgrupadosPorDreUseCase obterDadosDeLeituraComunicadosAgrupadosPorDreUseCase)
        {
            return Ok(await obterDadosDeLeituraComunicadosAgrupadosPorDreUseCase.Executar(notificacaoId, modoVisualizacao));
        }

        [HttpGet("leitura/modalidade")]
        [AllowAnonymous]
        //[ChaveIntegracaoFiltro]]
        public async Task<ObjectResult> ObterDadosDeLeituraComunicadosModalidade([FromQuery] string codigoDre, [FromQuery] string codigoUe, [FromQuery] long notificacaoId, ModoVisualizacao modoVisualizacao, [FromServices] IObterDadosDeLeituraModalidadeUseCase obterDadosDeLeituraModalidade)
        {
            return Ok(await obterDadosDeLeituraModalidade.Executar(codigoDre, codigoUe, notificacaoId, modoVisualizacao));
        }
        [HttpGet("leitura/turma")]
        [AllowAnonymous]
        //[ChaveIntegracaoFiltro]]
        public async Task<ObjectResult> ObterDadosDeLeituraComunicadosTurma([FromQuery] string codigoDre, [FromQuery] string codigoUe, [FromQuery] long notificacaoId, [FromQuery] short[] modalidade, [FromQuery] long codigoTurma, [FromQuery] ModoVisualizacao modoVisualizacao, [FromServices] IObterDadosDeLeituraTurmaUseCase obterDadosDeLeituraTurma)
        {
            return Ok(await obterDadosDeLeituraTurma.Executar(codigoDre, codigoUe, notificacaoId, modalidade, codigoTurma, modoVisualizacao));
        }
    }
}