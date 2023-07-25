using MediatR;
using Newtonsoft.Json;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class AtualizarDadosResponsavelEolCommandHandler : IRequestHandler<AtualizarDadosResponsavelEolCommand, bool>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public AtualizarDadosResponsavelEolCommandHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<bool> Handle(AtualizarDadosResponsavelEolCommand request, CancellationToken cancellationToken)
        {
            var dadosResponsavel =  new DadosResponsavelAlunoResumido() {CodigoAluno = request.CodigoAluno,
                                                                         Cpf = request.Cpf.ToString(),
                                                                         Email = request.Email,
                                                                         DataNascimento = request.DataNascimentoResponsavel,
                                                                         NomeMae = request.NomeMae,
                                                                         DDDCelular = request.DDD,
                                                                         NumeroCelular = request.Celular };

            var httpClient = httpClientFactory.CreateClient("servicoApiEolChave");
            var body = JsonConvert.SerializeObject(dadosResponsavel, UtilJson.ObterConfigConverterNulosEmVazio());

            var url = $"alunos/responsaveis/{request.Cpf}/alunos/{request.CodigoAluno}";
            var resposta = await httpClient.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
            Console.WriteLine(body);
            if (resposta.IsSuccessStatusCode && resposta.StatusCode != HttpStatusCode.NoContent)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                Console.WriteLine(body);
                Console.WriteLine(json);
                SentrySdk.CaptureMessage(json);

                if (json.ToLower().Contains("false"))
                    throw new Exception($"Não foi possível atualizar os dados cadastrais do cpf {request.Cpf}. Retorno: {json}");

                return true;
            }
            else throw new Exception($"Não foi possível atualizar os dados cadastrais do cpf {request.Cpf}.");
        }
    }
}
