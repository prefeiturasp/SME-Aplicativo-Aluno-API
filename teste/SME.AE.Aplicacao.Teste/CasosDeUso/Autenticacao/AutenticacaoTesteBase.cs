using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SME.AE.Api.Controllers;
using SME.AE.Aplicacao.Comum.Modelos;
using System.Security.Claims;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Autenticacao
{
    public abstract class AutenticacaoTesteBase : BaseTeste
    {
        protected AutenticacaoController Controller => new AutenticacaoController();

        protected static RespostaApi Sucesso() => RespostaApi.Sucesso();

        protected static RespostaApi Falha() => RespostaApi.Falha("Falha no caso de uso");

        protected static ControllerContext ContextoComUsuario(string nome)
        {
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, nome) }, "teste"))
                }
            };
        }

        protected static ControllerContext ContextoComMediator(IMediator mediator)
        {
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = new ServiceCollection().AddSingleton(mediator).BuildServiceProvider()
                }
            };
        }
    }
}
