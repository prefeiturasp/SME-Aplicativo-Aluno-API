using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class AutenticacaoController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<RespostaApi>> AutenticarUsuario([FromQuery] string cpf, [FromQuery] string senha, [FromQuery] string dispositivoId,[FromServices] IAutenticarUsuarioUseCase autenticarUsuarioUseCase)
        {
            return Ok(await autenticarUsuarioUseCase.Executar(cpf, senha, dispositivoId));
        }

        [HttpPost("Logout")]
        [AllowAnonymous]
        public async Task<ActionResult<RespostaApi>> Logout([FromQuery] string cpf, [FromQuery] string dispositivoId)
        {
            return Ok(await LogoutUsuarioUseCase.Executar(Mediator, cpf, dispositivoId));
        }

        [HttpPost("PrimeiroAcesso")]
        public async Task<ActionResult<RespostaApi>> PrimeiroAcesso([FromBody] NovaSenhaDto novaSenhaDto, [FromServices]IPrimeiroAcessoUseCase criarUsuarioPrimeiroAcessoUseCase)
        {
            return Ok(await criarUsuarioPrimeiroAcessoUseCase.Executar(novaSenhaDto));
        }

        [HttpPost("AlterarEmailCelular")]
        public async Task<ActionResult<RespostaApi>> AlterarEmailCelular([FromBody] AlterarEmailCelularDto alterarEmailCelularDto, [FromServices]IAlterarEmailCelularUseCase alterarEmailCelularUseCase)
        {
            return Ok(await alterarEmailCelularUseCase.Executar(Mediator, alterarEmailCelularDto));
        }
        
        [HttpPut("Senha/Alterar")]
        public async Task<ActionResult<RespostaApi>> AlterarSenha([FromBody]SenhaDto senha, [FromServices] IAlterarSenhaUseCase alterarSenhaUseCase)
        {
            return await alterarSenhaUseCase.Executar(new AlterarSenhaDto(User.Identity.Name, senha.NovaSenha), senha.SenhaAntiga);
        }

        [HttpPut("Senha/Token")]
        [AllowAnonymous]
        public async Task<ActionResult<RespostaApi>> SolicitarRedefinicao([FromBody]GerarTokenDto gerarTokenDto, [FromServices]ISolicitarRedifinicaoSenhaUseCase solicitarRedifinicaoSenhaUseCase)
        {
            return await solicitarRedifinicaoSenhaUseCase.Executar(gerarTokenDto);
        }

        [HttpPut("Senha/Token/Validar")]
        [AllowAnonymous]
        public async Task<ActionResult<RespostaApi>> ValidarToken([FromBody]ValidarTokenDto validarTokenDto,[FromServices]IValidarTokenUseCase validarTokenUseCase)
        {
            return await validarTokenUseCase.Executar(validarTokenDto);
        }

        [HttpPut("Senha/Redefinir")]
        [AllowAnonymous]
        public async Task<ActionResult<RespostaApi>> RedefinirSenha([FromBody]RedefinirSenhaDto redefinirSenhaDto)
        {
            return RespostaApi.Sucesso("aqui vai o token de autenticacao");
        }
    }
}
