using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class AutenticacaoController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<RespostaApi>> AutenticarUsuario([FromBody] AutenticacaoDTO autenticacao,[FromServices] IAutenticarUsuarioUseCase autenticarUsuarioUseCase)
        {
            return Ok(await autenticarUsuarioUseCase.Executar(autenticacao.Cpf, autenticacao.Senha, autenticacao.DispositivoId));
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
        public async Task<ActionResult<RespostaApi>> RedefinirSenha([FromBody]RedefinirSenhaDto redefinirSenhaDto,[FromServices]IRedefinirSenhaUseCase redefinirSenhaUseCase)
        {
            return await redefinirSenhaUseCase.Executar(redefinirSenhaDto);
        }

        [HttpPut("Senha/ReiniciarSenha")]
        [AllowAnonymous]
        public async Task<ActionResult<RespostaApi>> ReiniciarSenha([FromBody] SolicitarReiniciarSenhaDto solicitarReiniciarSenhaDto, [FromServices] ISolicitarReiniciarSenhaUseCase solicitarReiniciarSenhaUseCase)
        {
            return await solicitarReiniciarSenhaUseCase.Executar(solicitarReiniciarSenhaDto);
        }

        [HttpGet("usuario/responsavel")]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterSituacaoUsuario([FromQuery] string cpf, [FromServices] IValidarUsuarioEhResponsavelDeAlunoUseCase validarUsuarioEhResponsavelDeAlunoUseCase)
        {
            return Ok(await validarUsuarioEhResponsavelDeAlunoUseCase.Executar(cpf));
        }
    }
}
