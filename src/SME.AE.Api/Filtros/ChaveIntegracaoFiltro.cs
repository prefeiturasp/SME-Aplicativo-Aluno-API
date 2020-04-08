﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.AE.Aplicacao.Comum.Config;

namespace SME.AE.Api.Filtros
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ChaveIntegracaoFiltro : Attribute, IAsyncActionFilter
    {
        private const string ChaveIntegracaoHeader = "x-integration-key";
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ChaveIntegracaoHeader, out var chaveRecebida) ||
                !chaveRecebida.Equals(VariaveisAmbiente.ChaveIntegracao))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}