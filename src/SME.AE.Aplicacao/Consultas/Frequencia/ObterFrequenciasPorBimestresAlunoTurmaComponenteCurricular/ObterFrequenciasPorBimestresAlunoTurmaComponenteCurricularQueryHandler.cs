using MediatR;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum;
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
    public class ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQueryHandler : IRequestHandler<ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQuery, IEnumerable<FrequenciaAlunoDto>>
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ITurmaRepositorio turmaRepositorio;
        private readonly IParametrosEscolaAquiRepositorio parametrosEscolaAquiRepositorio;

        public ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQueryHandler(IHttpClientFactory httpClientFactory, ITurmaRepositorio turmaRepositorio,
            IParametrosEscolaAquiRepositorio parametrosEscolaAquiRepositorio)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.turmaRepositorio = turmaRepositorio;
            this.parametrosEscolaAquiRepositorio = parametrosEscolaAquiRepositorio;
        }

        public async Task<IEnumerable<FrequenciaAlunoDto>> Handle(ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<FrequenciaAlunoDto> frequencias;

            var httpClient = httpClientFactory.CreateClient("servicoApiSgp");
            var resposta = await httpClient.GetAsync($"v1/calendarios/frequencias/turmas/{request.TurmaCodigo}/alunos/{request.AlunoCodigo}/componentes-curriculares/{request.ComponenteCurricularId}?bimestres={string.Join("&bimestres=", request.Bimestres)}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                frequencias = JsonConvert.DeserializeObject<IEnumerable<FrequenciaAlunoDto>>(json);

                frequencias = await AtualizarCores(frequencias, request.TurmaCodigo);
            }
            else
            {
                throw new Exception($"Não foi possível localizar as frequências do aluno : {request.AlunoCodigo} da turma {request.TurmaCodigo}.");
            }

            return frequencias;
        }

        private async Task<IEnumerable<FrequenciaAlunoDto>> AtualizarCores(IEnumerable<FrequenciaAlunoDto> frequencias, string turmaCodigo)
        {
            var turmaModalidadeDeEnsino = await turmaRepositorio.ObterModalidadeDeEnsino(turmaCodigo);
            var parametros = turmaModalidadeDeEnsino.ModalidadeDeEnsino == ModalidadeDeEnsino.Infantil
                ? await parametrosEscolaAquiRepositorio.ObterParametros(FrequenciaAlunoFaixa.ObterChavesDosParametrosParaEnsinoInfantil())
                : await parametrosEscolaAquiRepositorio.ObterParametros(FrequenciaAlunoFaixa.ObterChavesDosParametros());

            var frequenciaAlunoCores = ObterFrequenciaAlunoCor(parametros);
            var frequenciaAlunoFaixas = ObterFrequenciaAlunoFaixa(parametros);

            List<FrequenciaAlunoDto> frequenciasAtualizada = new List<FrequenciaAlunoDto>();
            foreach (var frequencia in frequencias)
            {
                frequencia.CorDaFrequencia = turmaModalidadeDeEnsino.ModalidadeDeEnsino == ModalidadeDeEnsino.Infantil
                    ? DefinirCorDaFrequenciaParaEnsinoInfantil(frequencia.PercentualFrequencia, frequenciaAlunoCores, frequenciaAlunoFaixas)
                    : DefinirCorDaFrequencia(frequencia.PercentualFrequencia, frequenciaAlunoCores, frequenciaAlunoFaixas);

                frequenciasAtualizada.Add(frequencia);
            }

            return frequenciasAtualizada;
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

        private string DefinirCorDaFrequencia(double frequencia, IEnumerable<FrequenciaAlunoCor> frequenciaAlunoCores, IEnumerable<FrequenciaAlunoFaixa> frequenciaAlunoFaixas)
        {
            if (!frequenciaAlunoCores.Any() || !frequenciaAlunoFaixas.Any()) return FrequenciaAlunoCor.CorPadrao;
            var frequenciaEmAlertaFaixa = frequenciaAlunoFaixas.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoFaixa.FrequenciaEmAlertaFaixa)?.Faixa;
            var frequenciaRegularFaixa = frequenciaAlunoFaixas.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoFaixa.FrequenciaRegularFaixa)?.Faixa;

            string cor = null;
            switch (Math.Round((decimal)frequencia, 2))
            {
                case decimal n when (n < frequenciaEmAlertaFaixa.GetValueOrDefault()):
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaInsuficienteCor)?.Cor;
                    break;

                case decimal n when (n < frequenciaRegularFaixa.GetValueOrDefault()):
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaEmAlertaCor)?.Cor;
                    break;

                default:
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaRegularCor)?.Cor;
                    break;
            }

            return cor ?? FrequenciaAlunoCor.CorPadrao;
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
    }
}
