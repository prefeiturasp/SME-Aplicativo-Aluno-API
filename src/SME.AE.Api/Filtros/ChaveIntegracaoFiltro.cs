using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SME.AE.Comum;
using Microsoft.Extensions.DependencyInjection;

namespace SME.AE.Api.Filtros
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ChaveIntegracaoFiltro : Attribute, IAsyncActionFilter
    {
        private const string ChaveIntegracaoHeader = "x-integration-key";
        private VariaveisGlobaisOptions variaveisGlobais;
        

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            variaveisGlobais = context.HttpContext.RequestServices.GetService<VariaveisGlobaisOptions>();

            if (!context.HttpContext.Request.Headers.TryGetValue(ChaveIntegracaoHeader, out var chaveRecebida) ||
                !chaveRecebida.Equals(variaveisGlobais.ChaveIntegracao))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}