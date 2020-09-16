using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using System;
using System.Threading.Tasks;

namespace SME.AE.Api.Controllers
{
    public class TermosDeUsoController : ApiController
    {
        [HttpGet()]
        [AllowAnonymous]
        public async Task<ObjectResult> ObterUsuariosPorCpf(string cpf, [FromServices] IObterTermosDeUsoUseCase obterTermosDeUsoUseCase)
        {
            return Ok(await obterTermosDeUsoUseCase.Executar(cpf));
        }

        [HttpPost("registrar-aceite")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> RegistrarAceite(RegistrarAceiteDosTermosDeUsoDto aceite, [FromServices] IRegistrarAceiteDosTermosDeUsoUseCase registrarAceiteDosTermosDeUsoUseCase)
        {
            return Ok(await registrarAceiteDosTermosDeUsoUseCase.Executar(aceite));
        }


    }


}