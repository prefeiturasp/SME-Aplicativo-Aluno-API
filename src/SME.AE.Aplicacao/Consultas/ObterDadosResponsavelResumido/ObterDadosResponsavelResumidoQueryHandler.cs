using System;
using MediatR;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SME.AE.Comum.Excecoes;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterDadosResponsavelResumidoQueryHandler : IRequestHandler<ObterDadosResponsavelResumidoQuery, DadosResponsavelAlunoResumido>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ObterDadosResponsavelResumidoQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<DadosResponsavelAlunoResumido> Handle(ObterDadosResponsavelResumidoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var httpClient = httpClientFactory.CreateClient("servicoApiEolChave");
                var url = $"alunos/responsaveis/{request.CpfResponsavel}/resumido";

                var resposta = await httpClient.GetAsync(url);
                if (resposta.IsSuccessStatusCode)
                {
                    var json = await resposta.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<DadosResponsavelAlunoResumido>(json);
                }
                else
                {
                    throw new Exception($"Não foi possível obter dados do responsável");
                }
            }
            catch (Exception e)
            {
                throw new NegocioException(e.Message);
            }
        }
    }
}
