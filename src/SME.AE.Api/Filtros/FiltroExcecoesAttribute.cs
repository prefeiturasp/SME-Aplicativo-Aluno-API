﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Excecoes;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace SME.AE.Api.Filtros
{
    public class FiltroExcecoesAttribute : ExceptionFilterAttribute
    {
        public readonly string sentryDSN;

        public FiltroExcecoesAttribute(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            sentryDSN = VariaveisAmbiente.SentryDsn;
        }

        public override void OnException(ExceptionContext context)
        {
            using (SentrySdk.Init(sentryDSN))
            {
                var internalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList?.Where(c => c.AddressFamily == AddressFamily.InterNetwork).ToString();

                SentrySdk.AddBreadcrumb($"{Environment.MachineName ?? string.Empty} - {internalIP ?? string.Empty }", "Machine Identification");

                SentrySdk.CaptureException(context.Exception);
            }

            context.Result = context.Exception switch
            {
                NegocioException negocioException => new ResultadoBaseResult(context.Exception.Message, negocioException.StatusCode),
                ValidacaoException validacaoException => TratarValidacaoException(validacaoException),
                _ => new ResultadoBaseResult("Ocorreu um erro interno. Favor contatar o Suporte"),
            };

            base.OnException(context);
        }

        private static ResultadoBaseResult TratarValidacaoException(ValidacaoException validacaoException)
        {
            if (!validacaoException.Errors.Any())
                return new ResultadoBaseResult(validacaoException.Message);

            return new ResultadoBaseResult(RespostaApi.Falha(validacaoException.Errors));
        }
    }
}