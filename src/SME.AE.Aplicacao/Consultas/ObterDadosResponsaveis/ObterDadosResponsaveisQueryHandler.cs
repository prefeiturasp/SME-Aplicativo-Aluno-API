﻿using MediatR;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterDadosResponsaveisQueryHandler : IRequestHandler<ObterDadosResponsaveisQuery, IEnumerable<RetornoUsuarioEol>>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ObterDadosResponsaveisQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<RetornoUsuarioEol>> Handle(ObterDadosResponsaveisQuery request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("servicoApiEolChave");
            var url = $"alunos/responsaveis/{request.CpfResponsavel}";

            var resposta = await httpClient.GetAsync(url);
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<RetornoUsuarioEol>>(json);
            }
            else
            {
                throw new System.Exception($"Não foi possível obter dados do responsável");
            }
        }
    }
}
