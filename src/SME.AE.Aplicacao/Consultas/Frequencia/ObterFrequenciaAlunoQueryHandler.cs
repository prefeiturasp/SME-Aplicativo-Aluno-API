using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno.PorComponenteCurricular;
using SME.AE.Aplicacao.Consultas.Frequencia.PorComponenteCurricular;
using SME.AE.Comum.Excecoes;
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
        private readonly IFrequenciaAlunoCorRepositorio _frequenciaAlunoCorRepositorio;
        private readonly ITurmaRepositorio _turmaRepositorio;

        public ObterFrequenciaAlunoQueryHandler(IFrequenciaAlunoRepositorio frequenciaAlunoRepositorio, IFrequenciaAlunoCorRepositorio frequenciaAlunoCorRepositorio,
            ITurmaRepositorio turmaRepositorio)
        {
            _frequenciaAlunoRepositorio = frequenciaAlunoRepositorio ?? throw new System.ArgumentNullException(nameof(frequenciaAlunoRepositorio));
            _frequenciaAlunoCorRepositorio = frequenciaAlunoCorRepositorio;
            _turmaRepositorio = turmaRepositorio;
        }

        public async Task<FrequenciaAlunoPorComponenteCurricularResposta> Handle(ObterFrequenciaAlunoPorComponenteCurricularQuery request, CancellationToken cancellationToken)
        {
            var obterFrequenciaAlunoPorComponenteCurricular = _frequenciaAlunoRepositorio.ObterFrequenciaAlunoPorComponenteCurricularAsync(request.AnoLetivo, request.CodigoUe, request.CodigoTurma, request.CodigoAluno, request.CodigoComponenteCurricular);
            var obterFrequenciaAlunoCores = _frequenciaAlunoCorRepositorio.ObterAsync();
            var obterTurmaModalidadeDeEnsino = _turmaRepositorio.ObterModalidadeDeEnsinoAsync(request.CodigoTurma);
            await Task.WhenAll(obterFrequenciaAlunoPorComponenteCurricular, obterFrequenciaAlunoCores, obterTurmaModalidadeDeEnsino);

            var frequenciaAluno = obterFrequenciaAlunoPorComponenteCurricular.Result;
            var frequenciaAlunoCores = obterFrequenciaAlunoCores.Result;
            var turmaModalidadeDeEnsino = obterTurmaModalidadeDeEnsino.Result;

            if (frequenciaAluno is null)
            {
                throw new NegocioException("Não foi possível obter a frequência do aluno.");
            }

            if (turmaModalidadeDeEnsino is null)
            {
                throw new NegocioException("Não foi possível obter a modalidade de ensino do aluno.");
            }

            var modalidadeDeEnsinoInfantil = turmaModalidadeDeEnsino.ModalidadeDeEnsino == ModalidadeDeEnsino.Infantil;
            DefinirCoresDasFrequenciasPorComponenteCurricular(frequenciaAluno, frequenciaAlunoCores, modalidadeDeEnsinoInfantil);
            return frequenciaAluno;
        }

        public async Task<FrequenciaAlunoResposta> Handle(ObterFrequenciaAlunoQuery request, CancellationToken cancellationToken)
        {
            var obterFrequenciaDoAluno = _frequenciaAlunoRepositorio.ObterFrequenciaAlunoAsync(request.AnoLetivo, request.CodigoUe, request.CodigoTurma, request.CodigoAluno);
            var obterFrequenciaAlunoCores = _frequenciaAlunoCorRepositorio.ObterAsync();
            var obterTurmaModalidadeDeEnsino = _turmaRepositorio.ObterModalidadeDeEnsinoAsync(request.CodigoTurma);
            await Task.WhenAll(obterFrequenciaDoAluno, obterFrequenciaAlunoCores, obterTurmaModalidadeDeEnsino);

            var frequenciaDoAluno = obterFrequenciaDoAluno.Result;
            var turmaModalidadeDeEnsino = obterTurmaModalidadeDeEnsino.Result;

            if (frequenciaDoAluno is null)
            {
                throw new NegocioException("Não foi possível obter a frequência do aluno.");
            }

            if (turmaModalidadeDeEnsino is null)
            {
                throw new NegocioException("Não foi possível obter a modalidade de ensino do aluno.");
            }

            frequenciaDoAluno.CorDaFrequencia = turmaModalidadeDeEnsino.ModalidadeDeEnsino == ModalidadeDeEnsino.Infantil
                    ? DefinirCorDaFrequenciaParaEnsinoInfantil(frequenciaDoAluno.Frequencia, obterFrequenciaAlunoCores.Result)
                    : DefinirCoresDaFrequencia(frequenciaDoAluno.Frequencia, obterFrequenciaAlunoCores.Result);

            return frequenciaDoAluno;
        }

        private void DefinirCoresDasFrequenciasPorComponenteCurricular(FrequenciaAlunoPorComponenteCurricularResposta frequenciaAluno, IEnumerable<FrequenciaAlunoCor> frequenciaAlunoCores, bool ensinoInfantil)
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
                frequenciaAlunoPorBimestre.CorDaFrequencia = ensinoInfantil
                    ? DefinirCorDaFrequenciaParaEnsinoInfantil(frequenciaAlunoPorBimestre.Frequencia, frequenciaAlunoCores)
                    : DefinirCoresDaFrequencia(frequenciaAlunoPorBimestre.Frequencia, frequenciaAlunoCores);
            }
        }

        private string DefinirCoresDaFrequencia(decimal frequencia, IEnumerable<FrequenciaAlunoCor> frequenciaAlunoCores)
        {
            string cor = null;
            switch (frequencia)
            {
                case decimal n when (n < 75.00m):
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaInsuficiente)?.Cor;
                    break;

                case decimal n when (n <= 79.99m && n >= 75.00m):
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaEmAlerta)?.Cor;
                    break;

                case decimal n when (n >= 80.00m):
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaRegular)?.Cor;
                    break;
            }

            return cor ?? FrequenciaAlunoCor.CorPadrao;
        }

        private string DefinirCorDaFrequenciaParaEnsinoInfantil(decimal frequencia, IEnumerable<FrequenciaAlunoCor> frequenciaAlunoCores)
        {
            string cor = null;
            switch (frequencia)
            {
                case decimal n when (n < 60.00m):
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaInsuficiente)?.Cor;
                    break;

                case decimal n when (n <= 74.99m && n >= 60.00m):
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaEmAlerta)?.Cor;
                    break;

                case decimal n when (n >= 75.00m):
                    cor = frequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaRegular)?.Cor;
                    break;
            }

            return cor ?? FrequenciaAlunoCor.CorPadrao;
        }
    }
}