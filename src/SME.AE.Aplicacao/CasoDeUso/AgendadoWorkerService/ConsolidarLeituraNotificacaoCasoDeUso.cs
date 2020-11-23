using Microsoft.Practices.ObjectBuilder2;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            var usuariosAlunos = await ObterUsuariosAlunos();
            var comunicadosAtivos = await ObterComunicadosAtivos();
            var usuariosAlunosNotificacoesApp = await ObterUsuariosAlunosNotificacoesApp();

            await ConsolidarComunicadosUsuariosAlunos(usuariosAlunos, comunicadosAtivos, usuariosAlunosNotificacoesApp);
            await workerProcessoAtualizacaoRepositorio.IncluiOuAtualizaUltimaAtualizacao("ConsolidarLeituraNotificacao");
        }

        private async Task<IEnumerable<UsuarioAlunoNotificacaoApp>> ObterUsuariosAlunosNotificacoesApp()
        {
            var usuariosAlunosNotificacoesApp = 
                (await consolidarLeituraNotificacaoRepositorio.ObterUsuariosAlunosNotificacoesApp())
                .ToArray();
            return usuariosAlunosNotificacoesApp;
        }

        private async Task ConsolidarComunicadosUsuariosAlunos(IEnumerable<ResponsavelAlunoEOLDto> usuariosAlunos, IEnumerable<ComunicadoSgpDto> comunicadosAtivos, IEnumerable<UsuarioAlunoNotificacaoApp> usuariosAlunosNotificacoesApp)
        {
            comunicadosAtivos
                .AsParallel()
                .ForAll(comunicado => {
                    var consolidacaoNotificacoes = ConsolidarComunicado(comunicado, usuariosAlunos, usuariosAlunosNotificacoesApp);
                    consolidarLeituraNotificacaoRepositorio.SalvarConsolidacaoNotificacoesEmBatch(consolidacaoNotificacoes).Wait();
                });
            await Task.CompletedTask;
        }

        private IEnumerable<ConsolidacaoNotificacaoDto> ConsolidarComunicado(ComunicadoSgpDto comunicado, IEnumerable<ResponsavelAlunoEOLDto> usuariosAlunos, IEnumerable<UsuarioAlunoNotificacaoApp> usuariosAlunosNotificacoesApp)
        {
            var alunosComunicado =
                ObterAlunosDoComunicado(comunicado, usuariosAlunos)
                .Distinct()
                .ToArray()
                .AsParallel()
                ;

            var smeConsolidado = new ConsolidacaoNotificacaoDto[] { 
                ConsolidaNotificacoes(
                    new ConsolidacaoNotificacaoDto {
                        NotificacaoId = comunicado.Id,
                        AnoLetivo = comunicado.AnoLetivo,
                        DreCodigo = "",
                        UeCodigo = ""
                    },
                    alunosComunicado,
                    usuariosAlunosNotificacoesApp
                )
            };

            var dreConsolidado =
                alunosComunicado
                        .GroupBy(
                                responsavelChave => responsavelChave.CodigoDre,
                                responsavelValor => responsavelValor,
                                (chave, alunosDre) => ConsolidaNotificacoes(
                                    new ConsolidacaoNotificacaoDto {
                                        NotificacaoId = comunicado.Id,
                                        AnoLetivo = comunicado.AnoLetivo,
                                        DreCodigo = alunosDre.First().CodigoDre,
                                        UeCodigo = "",
                                    },
                                    alunosDre,
                                    usuariosAlunosNotificacoesApp
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
                                        NotificacaoId = comunicado.Id,
                                        AnoLetivo = comunicado.AnoLetivo,
                                        DreCodigo = alunosUe.First().CodigoDre,
                                        UeCodigo = alunosUe.First().CodigoUe,
                                    },
                                    alunosUe,
                                    usuariosAlunosNotificacoesApp
                                )
                            );

            var tudoConsolidado =
                smeConsolidado
                .Union(dreConsolidado)
                .Union(ueConsolidado)
                .ToArray();

            return tudoConsolidado;
        }

        private ConsolidacaoNotificacaoDto ConsolidaNotificacoes(ConsolidacaoNotificacaoDto consolidacao, IEnumerable<ResponsavelAlunoEOLDto> alunosComunicado, IEnumerable<UsuarioAlunoNotificacaoApp> usuariosAlunosNotificacoesApp)
        {
            var comApp = alunosComunicado
                .AsParallel()
                .Where(aluno =>
                    usuariosAlunosNotificacoesApp
                    .Any(uan => uan.CpfResponsavel == aluno.CpfResponsavel)
                );

            var responsaveisTotal = alunosComunicado
                .AsParallel()
                .Select(aluno => aluno.CpfResponsavel)
                .Distinct()
                .Count();

            var alunosTotal = alunosComunicado
                .AsParallel()
                .Select(aluno => aluno.CodigoAluno)
                .Distinct()
                .Count();

            var responsaveisComApp = alunosComunicado
                .AsParallel()
                .Where(aluno =>
                    usuariosAlunosNotificacoesApp
                    .Any(uan => uan.CpfResponsavel == aluno.CpfResponsavel)
                )
                .Select(aluno => aluno.CpfResponsavel)
                .Distinct()
                .Count();

            var alunosComApp = alunosComunicado
                .AsParallel()
                .Where(aluno =>
                    usuariosAlunosNotificacoesApp
                    .Any(uan => uan.CodigoAluno == aluno.CodigoAluno.ToString())
                )
                .Select(aluno => aluno.CodigoAluno)
                .Distinct()
                .Count();

            consolidacao.QuantidadeAlunosComApp = alunosComApp;
            consolidacao.QuantidadeAlunosSemApp = alunosTotal - alunosComApp;
            consolidacao.QuantidadeResponsaveisComApp = responsaveisComApp;
            consolidacao.QuantidadeResponsaveisSemApp = responsaveisTotal - responsaveisComApp;

            return consolidacao;
        }

        private IEnumerable<ResponsavelAlunoEOLDto> ObterAlunosDoComunicado(ComunicadoSgpDto comunicado, IEnumerable<ResponsavelAlunoEOLDto> usuariosAlunos)
        {
            var alunosComunicado = usuariosAlunos;

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
                return alunosComunicado;
            } else if(tipoCicloId.Any() && etapaEnsinoId.Any())
            {
                alunosComunicado = alunosComunicado.Where(aluno => tipoCicloId.Contains(aluno.CodigoCicloEnsino) && etapaEnsinoId.Contains(aluno.CodigoEtapaEnsino));
                return alunosComunicado;
            }
            return new ResponsavelAlunoEOLDto[] { };
        }

        private async Task<IEnumerable<ResponsavelAlunoEOLDto>> ObterUsuariosAlunos()
        {
            var responsaveisEOL =
                (await responsavelEOLRepositorio.ListarCpfResponsavelAlunoDaDreUeTurma())
                .AsParallel()
                .Where(resp => ValidacaoCpf.Valida(resp.CpfResponsavel.ToString("00000000000")))
                .ToArray();
            return responsaveisEOL;
        }

        private async Task<IEnumerable<ComunicadoSgpDto>> ObterComunicadosAtivos()
        {
            return (await consolidarLeituraNotificacaoSgpRepositorio.ObterComunicadosSgp())
                ///.Where(a => a.Id == 661)
                ;
        }

    }
}
