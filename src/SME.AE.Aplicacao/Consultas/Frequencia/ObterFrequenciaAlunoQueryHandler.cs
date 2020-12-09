using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno.PorComponenteCurricular;
using SME.AE.Aplicacao.Consultas.Frequencia.PorComponenteCurricular;
using SME.AE.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.Frequencia
{
    public class ObterFrequenciaAlunoQueryHandler : IRequestHandler<ObterFrequenciaAlunoPorComponenteCurricularQuery, FrequenciaAlunoPorComponenteCurricularResposta>,
                                                    IRequestHandler<ObterFrequenciaAlunoQuery, FrequenciaAlunoResposta>
    {
        private readonly IFrequenciaAlunoRepositorio _frequenciaAlunoRepositorio;
        private readonly ITurmaRepositorio _turmaRepositorio;
        private readonly IParametrosEscolaAquiRepositorio _parametrosEscolaAquiRepositorio;

        public ObterFrequenciaAlunoQueryHandler(IFrequenciaAlunoRepositorio frequenciaAlunoRepositorio, ITurmaRepositorio turmaRepositorio,
            IParametrosEscolaAquiRepositorio parametrosEscolaAquiRepositorio)
        {
            _frequenciaAlunoRepositorio = frequenciaAlunoRepositorio ?? throw new System.ArgumentNullException(nameof(frequenciaAlunoRepositorio));
            _turmaRepositorio = turmaRepositorio;
            _parametrosEscolaAquiRepositorio = parametrosEscolaAquiRepositorio;
        }

        public async Task<FrequenciaAlunoPorComponenteCurricularResposta> Handle(ObterFrequenciaAlunoPorComponenteCurricularQuery request, CancellationToken cancellationToken)
        {
            var obterFrequenciaAlunoPorComponenteCurricular = _frequenciaAlunoRepositorio.ObterFrequenciaAlunoPorComponenteCurricularAsync(request.AnoLetivo, request.CodigoUe, request.CodigoTurma, request.CodigoAluno, request.CodigoComponenteCurricular);
            var obterTurmaModalidadeDeEnsino = _turmaRepositorio.ObterModalidadeDeEnsino(request.CodigoTurma);
            await Task.WhenAll(obterFrequenciaAlunoPorComponenteCurricular, obterTurmaModalidadeDeEnsino);

            var frequenciaAluno = obterFrequenciaAlunoPorComponenteCurricular.Result;
            var turmaModalidadeDeEnsino = obterTurmaModalidadeDeEnsino.Result;

            if (frequenciaAluno is null)
            {
                throw new NegocioException("Não foi possível obter a frequência do aluno.");
            }

            if (turmaModalidadeDeEnsino is null)
            {
                throw new NegocioException("Não foi possível obter a modalidade de ensino do aluno.");
            }

            var obterFrequenciaAlunoCores = ObterFrequenciaAlunoCor(turmaModalidadeDeEnsino.ModalidadeDeEnsino);
            var obterFrequenciaAlunoFaixa = ObterFrequenciaAlunoFaixa(turmaModalidadeDeEnsino.ModalidadeDeEnsino);
            await Task.WhenAll(obterFrequenciaAlunoCores, obterFrequenciaAlunoFaixa);

            DefinirCoresDasFrequenciasPorComponenteCurricular(frequenciaAluno, turmaModalidadeDeEnsino.ModalidadeDeEnsino,
                obterFrequenciaAlunoCores.Result, obterFrequenciaAlunoFaixa.Result);
            return frequenciaAluno;
        }

        public async Task<FrequenciaAlunoResposta> Handle(ObterFrequenciaAlunoQuery request, CancellationToken cancellationToken)
        {
            var obterFrequenciaDoAluno = _frequenciaAlunoRepositorio.ObterFrequenciaAlunoAsync(request.AnoLetivo, request.CodigoUe, request.CodigoTurma, request.CodigoAluno);
            var obterTurmaModalidadeDeEnsino = _turmaRepositorio.ObterModalidadeDeEnsino(request.CodigoTurma);
            await Task.WhenAll(obterFrequenciaDoAluno, obterTurmaModalidadeDeEnsino);

            var frequenciaDoAluno = obterFrequenciaDoAluno.Result;
            var turmaModalidadeDeEnsino = obterTurmaModalidadeDeEnsino.Result;

            if (frequenciaDoAluno is null)
            {
                throw new NegocioException("Não existem registros de frequência para o aluno informado.");
            }

            if (turmaModalidadeDeEnsino is null)
            {
                throw new NegocioException("Não foi possível obter a modalidade de ensino do aluno.");
            }

            var obterFrequenciaAlunoCores = ObterFrequenciaAlunoCor(turmaModalidadeDeEnsino.ModalidadeDeEnsino);
            var obterFrequenciaAlunoFaixa = ObterFrequenciaAlunoFaixa(turmaModalidadeDeEnsino.ModalidadeDeEnsino);
            await Task.WhenAll(obterFrequenciaAlunoCores, obterFrequenciaAlunoFaixa);

            frequenciaDoAluno.CorDaFrequencia = turmaModalidadeDeEnsino.ModalidadeDeEnsino == ModalidadeDeEnsino.Infantil
                ? DefinirCorDaFrequenciaParaEnsinoInfantil(frequenciaDoAluno.Frequencia, obterFrequenciaAlunoCores.Result, obterFrequenciaAlunoFaixa.Result)
                : DefinirCorDaFrequenciaGlobal(frequenciaDoAluno.Frequencia, obterFrequenciaAlunoCores.Result, obterFrequenciaAlunoFaixa.Result);

            return frequenciaDoAluno;
        }

        private void DefinirCoresDasFrequenciasPorComponenteCurricular(FrequenciaAlunoPorComponenteCurricularResposta frequenciaAluno, ModalidadeDeEnsino modalidadeDeEnsino,
            IEnumerable<FrequenciaAlunoCor> frequenciaAlunoCores, IEnumerable<FrequenciaAlunoFaixa> frequenciaAlunoFaixas)
        {
            if (!frequenciaAlunoCores?.Any() ?? true)
            {
                foreach (var frequenciaAlunoPorBimestre in frequenciaAluno.FrequenciasPorBimestre)
                {
                    frequenciaAlunoPorBimestre.CorDaFrequencia = FrequenciaAlunoCor.CorPadrao;
                }
                return;
            }

            foreach (var frequenciaAlunoPorBimestre in frequenciaAluno.FrequenciasPorBimestre)
            {
                frequenciaAlunoPorBimestre.CorDaFrequencia = modalidadeDeEnsino == ModalidadeDeEnsino.Infantil
                    ? DefinirCorDaFrequenciaParaEnsinoInfantil(frequenciaAlunoPorBimestre.Frequencia, frequenciaAlunoCores, frequenciaAlunoFaixas)
                    : DefinirCorDaFrequencia(frequenciaAlunoPorBimestre.Frequencia, frequenciaAlunoCores, frequenciaAlunoFaixas);
            }
        }

        private string DefinirCorDaFrequenciaGlobal(decimal frequencia, IEnumerable<FrequenciaAlunoCor> frequenciaAlunoCores, IEnumerable<FrequenciaAlunoFaixa> frequenciaAlunoFaixas)
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

        private string DefinirCorDaFrequencia(decimal frequencia, IEnumerable<FrequenciaAlunoCor> frequenciaAlunoCores, IEnumerable<FrequenciaAlunoFaixa> frequenciaAlunoFaixas)
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

        private string DefinirCorDaFrequenciaParaEnsinoInfantil(decimal frequencia, IEnumerable<FrequenciaAlunoCor> frequenciaAlunoCores, IEnumerable<FrequenciaAlunoFaixa> frequenciaAlunoFaixas)
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

        public async Task<IEnumerable<FrequenciaAlunoCor>> ObterFrequenciaAlunoCor(ModalidadeDeEnsino modalidadeDeEnsino)
        {
            var parametros = modalidadeDeEnsino == ModalidadeDeEnsino.Infantil
                ? await _parametrosEscolaAquiRepositorio.ObterParametros(FrequenciaAlunoCor.ObterChavesDosParametrosParaEnsinoInfantil())
                : await _parametrosEscolaAquiRepositorio.ObterParametros(FrequenciaAlunoCor.ObterChavesDosParametros());

            return parametros
                .Select(x => new FrequenciaAlunoCor
                {
                    Cor = x.Conteudo,
                    Frequencia = x.Chave
                })
                .ToList();
        }

        public async Task<IEnumerable<FrequenciaAlunoFaixa>> ObterFrequenciaAlunoFaixa(ModalidadeDeEnsino modalidadeDeEnsino)
        {
            var parametros = modalidadeDeEnsino == ModalidadeDeEnsino.Infantil
                ? await _parametrosEscolaAquiRepositorio.ObterParametros(FrequenciaAlunoFaixa.ObterChavesDosParametrosParaEnsinoInfantil())
                : await _parametrosEscolaAquiRepositorio.ObterParametros(FrequenciaAlunoFaixa.ObterChavesDosParametros());

            return parametros
                .Select(x => new FrequenciaAlunoFaixa
                {
                    Faixa = decimal.TryParse(x.Conteudo, out var faixa) ? faixa : default,
                    Frequencia = x.Chave
                })
                .ToList();
        }
    }
}