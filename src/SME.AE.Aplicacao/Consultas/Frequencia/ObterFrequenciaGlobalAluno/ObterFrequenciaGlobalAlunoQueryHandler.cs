using MediatR;
using Newtonsoft.Json;
using Sentry;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterFrequenciaGlobalAlunoQueryHandler : IRequestHandler<ObterFrequenciaGlobalAlunoQuery, FrequenciaGlobalDto>
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ITurmaRepositorio turmaRepositorio;
        private readonly IParametrosEscolaAquiRepositorio parametrosEscolaAquiRepositorio;

        public ObterFrequenciaGlobalAlunoQueryHandler(IHttpClientFactory httpClientFactory, ITurmaRepositorio turmaRepositorio,
            IParametrosEscolaAquiRepositorio parametrosEscolaAquiRepositorio)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.turmaRepositorio = turmaRepositorio ?? throw new ArgumentNullException(nameof(turmaRepositorio));
            this.parametrosEscolaAquiRepositorio = parametrosEscolaAquiRepositorio ?? throw new ArgumentNullException(nameof(parametrosEscolaAquiRepositorio));
        }

        public async Task<FrequenciaGlobalDto> Handle(ObterFrequenciaGlobalAlunoQuery request, CancellationToken cancellationToken)
        {
            FrequenciaGlobalDto frequenciaGlobal = new FrequenciaGlobalDto();
            var httpClient = httpClientFactory.CreateClient("servicoApiSgp");
            var resposta = await httpClient.GetAsync($"v1/calendarios/frequencias/alunos/{request.AlunoCodigo}/turmas/{request.TurmaCodigo}/geral");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                frequenciaGlobal.Frequencia = JsonConvert.DeserializeObject<double>(json);

                var turmaModalidadeDeEnsino = await turmaRepositorio.ObterModalidadeDeEnsino(request.TurmaCodigo);
                var parametros = turmaModalidadeDeEnsino.ModalidadeDeEnsino == ModalidadeDeEnsino.Infantil
                ? await parametrosEscolaAquiRepositorio.ObterParametros(FrequenciaAlunoCor.ObterChavesDosParametrosParaEnsinoInfantil())
                : await parametrosEscolaAquiRepositorio.ObterParametros(FrequenciaAlunoCor.ObterChavesDosParametros());

                var obterFrequenciaAlunoCores = ObterFrequenciaAlunoCor(parametros);
                var obterFrequenciaAlunoFaixa = ObterFrequenciaAlunoFaixa(parametros);

                frequenciaGlobal.CorDaFrequencia = turmaModalidadeDeEnsino.ModalidadeDeEnsino == ModalidadeDeEnsino.Infantil
                    ? DefinirCorDaFrequenciaParaEnsinoInfantil(frequenciaGlobal.Frequencia, obterFrequenciaAlunoCores, obterFrequenciaAlunoFaixa)
                    : DefinirCorDaFrequenciaGlobal(frequenciaGlobal.Frequencia, obterFrequenciaAlunoCores, obterFrequenciaAlunoFaixa);

            }
            else
            {
                SentrySdk.CaptureMessage(resposta.ReasonPhrase);
                return null;
            }

            return frequenciaGlobal;
        }

        public IEnumerable<FrequenciaAlunoCor> ObterFrequenciaAlunoCor(IEnumerable<ParametroEscolaAqui> parametros)
        {
            return parametros
                .Select(x => new FrequenciaAlunoCor
                {
                    Cor = x.Conteudo,
                    Frequencia = x.Chave
                })
                .ToList();
        }

        public IEnumerable<FrequenciaAlunoFaixa> ObterFrequenciaAlunoFaixa(IEnumerable<ParametroEscolaAqui> parametros)
        {
            return parametros
                .Select(x => new FrequenciaAlunoFaixa
                {
                    Faixa = decimal.TryParse(x.Conteudo, out var faixa) ? faixa : default,
                    Frequencia = x.Chave
                })
                .ToList();
        }

        private string DefinirCorDaFrequenciaParaEnsinoInfantil(double frequencia, IEnumerable<FrequenciaAlunoCor> frequenciaAlunoCores, IEnumerable<FrequenciaAlunoFaixa> frequenciaAlunoFaixas)
        {
            if (!frequenciaAlunoCores.Any() || !frequenciaAlunoFaixas.Any()) return FrequenciaAlunoCor.CorPadrao;
            var frequenciaEmAlertaFaixa = frequenciaAlunoFaixas.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoFaixa.EnsinoInfantilFrequenciaEmAlertaFaixa)?.Faixa;
            var frequenciaRegularFaixa = frequenciaAlunoFaixas.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoFaixa.EnsinoInfantilFrequenciaRegularFaixa)?.Faixa;

            string cor = null;
            switch (Math.Round((decimal)frequencia, 2))
            {
                case decimal n when (n < frequenciaEmAlertaFaixa.GetValueOrDefault() / 100):
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.EnsinoInfantilFrequenciaInsuficienteCor)?.Cor;
                    break;

                case decimal n when (n < frequenciaRegularFaixa.GetValueOrDefault() / 100):
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.EnsinoInfantilFrequenciaEmAlertaCor)?.Cor;
                    break;

                default:
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.EnsinoInfantilFrequenciaRegularCor)?.Cor;
                    break;
            }

            return cor ?? FrequenciaAlunoCor.CorPadrao;
        }

        private string DefinirCorDaFrequenciaGlobal(double frequencia, IEnumerable<FrequenciaAlunoCor> frequenciaAlunoCores, IEnumerable<FrequenciaAlunoFaixa> frequenciaAlunoFaixas)
        {
            if (!frequenciaAlunoCores.Any() || !frequenciaAlunoFaixas.Any()) return FrequenciaAlunoCor.CorPadrao;
            var frequenciaEmAlertaFaixa = frequenciaAlunoFaixas.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoFaixa.FrequenciaEmAlertaFaixa)?.Faixa;
            var frequenciaRegularFaixa = frequenciaAlunoFaixas.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoFaixa.FrequenciaRegularFaixa)?.Faixa;

            string cor = null;
            switch (Math.Round((decimal)frequencia, 2))
            {
                case decimal n when (n < frequenciaEmAlertaFaixa.GetValueOrDefault() / 100):
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaInsuficienteCor)?.Cor;
                    break;

                case decimal n when (n < frequenciaRegularFaixa.GetValueOrDefault() / 100):
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaEmAlertaCor)?.Cor;
                    break;

                default:
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaRegularCor)?.Cor;
                    break;
            }

            return cor ?? FrequenciaAlunoCor.CorPadrao;
        }
    }
}
