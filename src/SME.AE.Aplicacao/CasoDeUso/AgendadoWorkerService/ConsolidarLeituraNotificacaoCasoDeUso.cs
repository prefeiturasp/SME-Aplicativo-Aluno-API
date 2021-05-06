using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum.Utilitarios;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ConsolidarLeituraNotificacaoCasoDeUso
    {
        private readonly IResponsavelEOLRepositorio responsavelEOLRepositorio;
        private readonly IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio;
        private readonly IConsolidarLeituraNotificacaoRepositorio consolidarLeituraNotificacaoRepositorio;
        private readonly IConsolidarLeituraNotificacaoSgpRepositorio consolidarLeituraNotificacaoSgpRepositorio;

        public ConsolidarLeituraNotificacaoCasoDeUso(
                                            IResponsavelEOLRepositorio responsavelEOLRepositorio,
                                            IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio,
                                            IConsolidarLeituraNotificacaoRepositorio consolidarLeituraNotificacaoRepositorio,
                                            IConsolidarLeituraNotificacaoSgpRepositorio consolidarLeituraNotificacaoSgpRepositorio
                                            )
        {
            this.responsavelEOLRepositorio = responsavelEOLRepositorio ?? throw new ArgumentNullException(nameof(responsavelEOLRepositorio));
            this.workerProcessoAtualizacaoRepositorio = workerProcessoAtualizacaoRepositorio ?? throw new ArgumentNullException(nameof(workerProcessoAtualizacaoRepositorio));
            this.consolidarLeituraNotificacaoRepositorio = consolidarLeituraNotificacaoRepositorio ?? throw new ArgumentNullException(nameof(consolidarLeituraNotificacaoRepositorio));
            this.consolidarLeituraNotificacaoSgpRepositorio = consolidarLeituraNotificacaoSgpRepositorio ?? throw new ArgumentNullException(nameof(consolidarLeituraNotificacaoSgpRepositorio));
        }

        public async Task ExecutarAsync()
        {
            var comunicadosAtivos = await ObterComunicadosAtivos();
            var usuariosAlunos = await ObterUsuariosAlunos();
            await ConsolidarComunicadosUsuariosAlunos(usuariosAlunos, comunicadosAtivos);
            await workerProcessoAtualizacaoRepositorio.IncluiOuAtualizaUltimaAtualizacao("ConsolidarLeituraNotificacao");
        }

        private async Task<IEnumerable<UsuarioAlunoNotificacaoApp>> ObterUsuariosAlunosNotificacoesApp()
        {
            var usuariosAlunosNotificacoesApp =
                (await consolidarLeituraNotificacaoRepositorio.ObterUsuariosAlunosNotificacoesApp())
                .ToArray();
            return usuariosAlunosNotificacoesApp;
        }

        private async Task ConsolidarComunicadosUsuariosAlunos(IEnumerable<ResponsavelAlunoEOLDto> usuariosAlunos, IEnumerable<ComunicadoSgpDto> comunicadosAtivos)
        {
            var listasParaGravar = new ConcurrentQueue<ConsolidacaoNotificacaoDto[]>();
            var taskParaGravar = Task.CompletedTask;
            var taskSemaforo = new object();

            comunicadosAtivos
                .AsParallel()
                .GroupBy(
                    comunicadoChave => comunicadoChave.Id,
                    comunicadoLista => comunicadoLista,
                    (id, comunicados) =>
                    {

                        var alunosComunidado = comunicados
                            .SelectMany(comunicado => ObterAlunosDoComunicado(comunicado, usuariosAlunos))
                            .Distinct()
                            .ToArray();

                        var umComunicado = comunicados.First();

                        return new { id, umComunicado.AnoLetivo, alunosComunidado, TemUe = !String.IsNullOrWhiteSpace(umComunicado.CodigoUe) };
                    }
                )
                .ForAll(comunicado =>
                {
                    var consolidacaoNotificacoes = ConsolidarComunicado(comunicado.id, comunicado.AnoLetivo, comunicado.alunosComunidado, comunicado.TemUe);

                    while (listasParaGravar.Count > 0) Task.Delay(100).Wait();

                    listasParaGravar.Enqueue(consolidacaoNotificacoes.ToArray());
                    lock (taskSemaforo)
                    {
                        if (taskParaGravar.IsCompleted)
                        {
                            taskParaGravar = taskParaGravar.ContinueWith(async (_) =>
                            {
                                while (listasParaGravar.TryDequeue(out var consolidacoes))
                                {
                                    await consolidarLeituraNotificacaoRepositorio.SalvarConsolidacaoNotificacoesEmBatch(consolidacoes);
                                }
                            });
                        }
                    }
                });

            await taskParaGravar;
        }

        private IEnumerable<ConsolidacaoNotificacaoDto> ConsolidarComunicado(long comunicadoId, short AnoLetivo, IEnumerable<ResponsavelAlunoEOLDto> usuariosAlunos, bool temUe)
        {
            var alunosComunicado =
                usuariosAlunos
                .AsParallel();

            var smeConsolidado = new ConsolidacaoNotificacaoDto[] {
                ConsolidaNotificacoes(
                    new ConsolidacaoNotificacaoDto {
                        NotificacaoId = comunicadoId,
                        AnoLetivo = AnoLetivo,
                        DreCodigo = "",
                        UeCodigo = "",
                        ModalidadeCodigo = 0,
                        TurmaCodigo = 0,
                        Turma = ""
                    },
                    alunosComunicado
                )
            };

            var dreConsolidado =
                alunosComunicado
                .GroupBy(
                        responsavelChave => responsavelChave.CodigoDre,
                        responsavelValor => responsavelValor,
                        (chave, alunosDre) => ConsolidaNotificacoes(
                            new ConsolidacaoNotificacaoDto
                            {
                                NotificacaoId = comunicadoId,
                                AnoLetivo = AnoLetivo,
                                DreCodigo = alunosDre.First().CodigoDre,
                                UeCodigo = "",
                                ModalidadeCodigo = 0,
                                TurmaCodigo = 0,
                                Turma = ""
                            },
                            alunosDre
                        )
                    );

            var ueConsolidado =
                alunosComunicado
                .GroupBy(
                        responsavelChave => responsavelChave.CodigoUe,
                        responsavelValor => responsavelValor,
                        (chave, alunosUe) => ConsolidaNotificacoes(
                            new ConsolidacaoNotificacaoDto
                            {
                                NotificacaoId = comunicadoId,
                                AnoLetivo = AnoLetivo,
                                DreCodigo = alunosUe.First().CodigoDre,
                                UeCodigo = alunosUe.First().CodigoUe,
                                ModalidadeCodigo = 0,
                                TurmaCodigo = 0,
                                Turma = ""
                            },
                            alunosUe
                        )
                    );

            var tudoConsolidado =
                smeConsolidado
                .Union(dreConsolidado)
                .Union(ueConsolidado);

            if (temUe)
            {
                var modalidadeConsolidado =
                    alunosComunicado
                    .GroupBy(
                            responsavelChave => new { responsavelChave.CodigoUe, responsavelChave.CodigoModalidadeTurma },
                            responsavelValor => responsavelValor,
                            (chave, alunosModalidade) => ConsolidaNotificacoes(
                                new ConsolidacaoNotificacaoDto
                                {
                                    NotificacaoId = comunicadoId,
                                    AnoLetivo = AnoLetivo,
                                    DreCodigo = alunosModalidade.First().CodigoDre,
                                    UeCodigo = alunosModalidade.First().CodigoUe,
                                    ModalidadeCodigo = alunosModalidade.First().CodigoModalidadeTurma,
                                    TurmaCodigo = 0,
                                    Turma = ""
                                },
                                alunosModalidade
                            )
                        );

                var turmaConsolidado =
                    alunosComunicado
                    .GroupBy(
                            responsavelChave => new { responsavelChave.CodigoUe, responsavelChave.CodigoModalidadeTurma, responsavelChave.CodigoTurma },
                            responsavelValor => responsavelValor,
                            (chave, alunosTurma) => ConsolidaNotificacoes(
                                new ConsolidacaoNotificacaoDto
                                {
                                    NotificacaoId = comunicadoId,
                                    AnoLetivo = AnoLetivo,
                                    DreCodigo = alunosTurma.First().CodigoDre,
                                    UeCodigo = alunosTurma.First().CodigoUe,
                                    ModalidadeCodigo = alunosTurma.First().CodigoModalidadeTurma,
                                    TurmaCodigo = alunosTurma.First().CodigoTurma,
                                    Turma = alunosTurma.First().Turma
                                },
                                alunosTurma
                            )
                        );

                tudoConsolidado =
                    tudoConsolidado
                    .Union(modalidadeConsolidado)
                    .Union(turmaConsolidado);
            }

            return
                tudoConsolidado.ToArray();
        }

        private ConsolidacaoNotificacaoDto ConsolidaNotificacoes(ConsolidacaoNotificacaoDto consolidacao, IEnumerable<ResponsavelAlunoEOLDto> alunosComunicado)
        {

            var comApp = alunosComunicado
                .Where(aluno => aluno.TemAppInstalado)
                .ToArray();

            var responsaveisTotal = alunosComunicado
                .Select(aluno => aluno.CpfResponsavel)
                .Distinct()
                .Count();

            var responsaveisComApp = comApp
                .Select(aluno => aluno.CpfResponsavel)
                .Distinct()
                .Count();

            var alunosTotal = alunosComunicado
                .Count();

            var alunosComApp = comApp
                .Count();

            consolidacao.QuantidadeAlunosComApp = alunosComApp;
            consolidacao.QuantidadeAlunosSemApp = alunosTotal - alunosComApp;
            consolidacao.QuantidadeResponsaveisComApp = responsaveisComApp;
            consolidacao.QuantidadeResponsaveisSemApp = responsaveisTotal - responsaveisComApp;

            return consolidacao;
        }

        private IEnumerable<ResponsavelAlunoEOLDto> ObterAlunosDoComunicado(ComunicadoSgpDto comunicado, IEnumerable<ResponsavelAlunoEOLDto> usuariosAlunos)
        {
            var alunosComunicado = usuariosAlunos
                .AsParallel();

            if (!string.IsNullOrWhiteSpace(comunicado.CodigoDre))
                alunosComunicado = alunosComunicado.Where(aluno => aluno.CodigoDre == comunicado.CodigoDre);

            if (!string.IsNullOrWhiteSpace(comunicado.CodigoUe))
                alunosComunicado = alunosComunicado.Where(aluno => aluno.CodigoUe == comunicado.CodigoUe);

            if (!string.IsNullOrWhiteSpace(comunicado.TurmaCodigo))
                alunosComunicado = alunosComunicado.Where(aluno => aluno.CodigoTurma.ToString() == comunicado.TurmaCodigo);

            if (!string.IsNullOrWhiteSpace(comunicado.AlunoCodigo))
                alunosComunicado = alunosComunicado.Where(aluno => aluno.CodigoAluno.ToString() == comunicado.AlunoCodigo);

            if (comunicado.Modalidade.HasValue)
                alunosComunicado = alunosComunicado.Where(aluno => aluno.CodigoModalidadeTurma == comunicado.Modalidade);

            var series = comunicado.SeriesResumidas.ToStringEnumerable();
            if (series.Any())
                alunosComunicado = alunosComunicado.Where(aluno => series.Contains(aluno.SerieResumida));

            var tipoEscolaId = comunicado.TipoEscolaId.ToShortEnumerable();
            var tipoCicloId = comunicado.TipoCicloId.ToShortEnumerable();
            var etapaEnsinoId = comunicado.EtapaEnsinoId.ToShortEnumerable();

            if (tipoEscolaId.Any())
            {
                alunosComunicado = alunosComunicado.Where(aluno => tipoEscolaId.Contains(aluno.CodigoTipoEscola));

                return
                    alunosComunicado
                    .Distinct()
                    .ToArray();

            }
            else if (tipoCicloId.Any() && etapaEnsinoId.Any())
            {
                alunosComunicado = alunosComunicado.Where(aluno => tipoCicloId.Contains(aluno.CodigoCicloEnsino) && etapaEnsinoId.Contains(aluno.CodigoEtapaEnsino));

                return
                    alunosComunicado
                    .Distinct()
                    .ToArray();

            }
            return new ResponsavelAlunoEOLDto[] { };
        }

        private async Task<IEnumerable<ResponsavelAlunoEOLDto>> ObterUsuariosAlunos()
        {
            var usuariosComApp =
                (await ObterUsuariosAlunosNotificacoesApp())
                .Select(usuario => usuario.CpfResponsavel)
                .Distinct()
                .OrderBy(u => u)
                .ToArray();

            var responsaveisEOL =
                (await responsavelEOLRepositorio.ListarCpfResponsavelAlunoDaDreUeTurma())
                .AsParallel()
                .Where(resp => ValidacaoCpf.Valida(resp.CpfResponsavel.ToString("00000000000")))
                .Select(usuarioAluno =>
                {
                    usuarioAluno
                        .TemAppInstalado =
                            Array
                            .BinarySearch(usuariosComApp, usuarioAluno.CpfResponsavel) >= 0;
                    return usuarioAluno;
                })
                .ToArray();
            return responsaveisEOL;
        }

        private async Task<IEnumerable<ComunicadoSgpDto>> ObterComunicadosAtivos()
        {
            return (await consolidarLeituraNotificacaoSgpRepositorio.ObterComunicadosSgp());
        }

    }
}
