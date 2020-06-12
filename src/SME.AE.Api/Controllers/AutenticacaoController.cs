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
        public async Task<ActionResult<RespostaApi>> PrimeiroAcesso([FromBody] NovaSenhaDto novaSenhaDto)
        {
            var validador = new NovaSenhaDtoValidator();

            var result = await validador.ValidateAsync(novaSenhaDto);

            var respostaAPI = new RespostaApi
            {
                Ok = result.IsValid
            };

            if (respostaAPI.Ok)
                return Ok(respostaAPI);

            respostaAPI.Erros = result.Errors.Select(x => x.ErrorMessage).ToArray();
            return StatusCode(400, respostaAPI);
        }

        [HttpPost("AlterarEmailTelefone")]
        public async Task<ActionResult<RespostaApi>> AlterarEmailTelefone([FromBody]AlterarEmailTelefoneDto alterarEmailTelefoneDto)
        {
            var validador = new AlterarEmailTelefoneDtoValidator();

            var result = await validador.ValidateAsync(alterarEmailTelefoneDto);

            var respostaApi = new RespostaApi
            {
                Ok = result.IsValid
            };

            if (respostaApi.Ok)
                return Ok(respostaApi);

            respostaApi.Erros = result.Errors.Select(x => x.ErrorMessage).ToArray();
            return StatusCode(400, respostaApi);
        }

        public class AlterarEmailTelefoneDto
        {
            public long Id { get; set; }
            public string Email { get; set; }
            public string Telefone { get; set; }
        }

        public class NovaSenhaDto
        {
            public long Id { get; set; }
            public string NovaSenha { get; set; }
        }

        public class AlterarEmailTelefoneDtoValidator : AbstractValidator<AlterarEmailTelefoneDto>
        {
            public AlterarEmailTelefoneDtoValidator()
            {
                RuleFor(x => x.Id).NotNull().GreaterThan(0);
                RuleFor(x => x.Telefone).NotNull().NotEmpty().Matches(@"(\(\d{2}\)\s)(\d{4,5}\-\d{4})").When(x => string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.Telefone));
                RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress().When(x => string.IsNullOrWhiteSpace(x.Telefone) || !string.IsNullOrWhiteSpace(x.Email));
            }
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
