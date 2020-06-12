using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;

namespace SME.AE.Api.Controllers
{
    public class AutenticacaoController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<RespostaApi>> AutenticarUsuario([FromQuery] string cpf, [FromQuery] string senha, [FromQuery] string dispositivoId)
        {
            try
            {
                return Ok(await AutenticarUsuarioUseCase.Executar(Mediator, cpf, senha, dispositivoId));
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost("Logout")]
        [AllowAnonymous]
        public async Task<ActionResult<RespostaApi>> Logout([FromQuery] string cpf, [FromQuery] string dispositivoId)
        {
            try
            {
                return Ok(await LogoutUsuarioUseCase.Executar(Mediator, cpf, dispositivoId));

            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost("PrimeiroAcesso")]
        public async Task<ActionResult<RespostaApi>> PrimeiroAcesso([FromBody] NovaSenhaDto novaSenhaDto, [FromServices]ICriarUsuarioPrimeiroAcessoUseCase criarUsuarioPrimeiroAcessoUseCase)
        {
            return Ok(await criarUsuarioPrimeiroAcessoUseCase.Executar(Mediator, novaSenhaDto));
        }

        [HttpPost("AlterarEmailCelular")]
        public async Task<ActionResult<RespostaApi>> AlterarEmailCelular([FromBody] AlterarEmailCelularDto alterarEmailCelularDto, [FromServices]IAlterarEmailCelularUseCase alterarEmailCelularUseCase)
        {
            return Ok(await alterarEmailCelularUseCase.Executar(Mediator, alterarEmailCelularDto));
        }
    }
}
