﻿using MediatR;
using Newtonsoft.Json;
using Sentry;
using SME.AE.Comum;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class EnviarAtualizacaoCadastralProdamCommandHandler : IRequestHandler<EnviarAtualizacaoCadastralProdamCommand, bool>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public EnviarAtualizacaoCadastralProdamCommandHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<bool> Handle(EnviarAtualizacaoCadastralProdamCommand request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("servicoAtualizacaoCadastralProdam");

            var body = JsonConvert.SerializeObject(request.ResponsavelDto, UtilJson.ObterConfigConverterNulosEmVazio());
            var resposta = await httpClient.PostAsync($"AtualizarResponsavelAluno", new StringContent(body, Encoding.UTF8, "application/json"));
            Console.WriteLine(body);
            if (resposta.IsSuccessStatusCode && resposta.StatusCode != HttpStatusCode.NoContent)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                Console.WriteLine(body);
                Console.WriteLine(json);
                SentrySdk.CaptureMessage(json);

                if (json.ToLower().Contains("false"))
                    throw new Exception($"Não foi possível atualizar os dados cadastrais do cpf {request.ResponsavelDto.CPF}. Retorno: {json}");

                return true;
            }
            else throw new Exception($"Não foi possível atualizar os dados cadastrais do cpf {request.ResponsavelDto.CPF}.");
        }
    }
}
