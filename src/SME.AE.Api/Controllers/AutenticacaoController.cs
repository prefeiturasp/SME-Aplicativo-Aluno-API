using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;

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
        [AllowAnonymous]
        public async Task<ActionResult<RespostaApi>> PrimeiroAcesso([FromBody] NovaSenhaDto novaSenhaDto)
        {
            var validador = new NovaSenhaDtoValidator();

            var result = await validador.ValidateAsync(novaSenhaDto);

            var respostaAPI = new RespostaApi();

            if (!result.IsValid)
            {
                respostaAPI.Erros = result.Errors.Select(x => x.ErrorMessage).ToArray();
                return StatusCode(400, respostaAPI);
            }

            respostaAPI.Ok = true;

            return Ok(respostaAPI);
        }

        public class NovaSenhaDto
        {
            public long Id { get; set; }
            public string NovaSenha { get; set; }
        }

        public class NovaSenhaDtoValidator : AbstractValidator<NovaSenhaDto>
        {
            public NovaSenhaDtoValidator()
            {
                RuleFor(x => x.Id).NotNull().GreaterThan(0);
                RuleFor(x => x.NovaSenha).MinimumLength(8)
                .MaximumLength(12)
                .Must(x => !x.Contains(" "))
                .Matches(@"(?=.*?[A-Z])(?=.*?[a-z])(?=((?=.*[!@#$\-%&/\\\[\]|*()_=+])|(?=.*?[0-9]+)))");
            }
        }
    }
}
