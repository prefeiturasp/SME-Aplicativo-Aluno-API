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
                ;

            var smeConsolidado = new ConsolidacaoNotificacaoDto[] { new ConsolidacaoNotificacaoDto {
                NotificacaoId = comunicado.Id,
                AnoLetivo = comunicado.AnoLetivo,
                DreCodigo = "",
                UeCodigo = "",
                QuantidadeAlunosSemApp = ObterAlunosSemApp(alunosComunicado, usuariosAlunosNotificacoesApp),
                QuantidadeResponsaveisSemApp = ObterResponsaveisSemApp(alunosComunicado, usuariosAlunosNotificacoesApp),
                QuantidadeAlunosSemLer = ObterAlunosSemLer(comunicado, alunosComunicado, usuariosAlunosNotificacoesApp),
                QuantidadeResponsaveisSemLer = ObterResponsaveisSemLer(comunicado, alunosComunicado, usuariosAlunosNotificacoesApp),
            } };

            var dreConsolidado =
                alunosComunicado
                        .GroupBy(
                                responsavelChave => responsavelChave.CodigoDre,
                                responsavelValor => responsavelValor,
                                (chave, alunosDre) => new ConsolidacaoNotificacaoDto {
                                    NotificacaoId = comunicado.Id,
                                    AnoLetivo = comunicado.AnoLetivo,
                                    DreCodigo = alunosDre.First().CodigoDre,
                                    UeCodigo = "",
                                    QuantidadeAlunosSemApp = ObterAlunosSemApp(alunosDre, usuariosAlunosNotificacoesApp),
                                    QuantidadeResponsaveisSemApp = ObterResponsaveisSemApp(alunosDre, usuariosAlunosNotificacoesApp),
                                    QuantidadeAlunosSemLer = ObterAlunosSemLer(comunicado, alunosDre, usuariosAlunosNotificacoesApp),
                                    QuantidadeResponsaveisSemLer = ObterResponsaveisSemLer(comunicado, alunosDre, usuariosAlunosNotificacoesApp),
                                }
                            ).ToArray();

            var ueConsolidado =
                alunosComunicado
                        .GroupBy(
                                responsavelChave => responsavelChave.CodigoUe,
                                responsavelValor => responsavelValor,
                                (chave, alunosUe) => new ConsolidacaoNotificacaoDto
                                {
                                    NotificacaoId = comunicado.Id,
                                    AnoLetivo = comunicado.AnoLetivo,
                                    DreCodigo = alunosUe.First().CodigoDre,
                                    UeCodigo = alunosUe.First().CodigoUe,
                                    QuantidadeAlunosSemApp = ObterAlunosSemApp(alunosUe, usuariosAlunosNotificacoesApp),
                                    QuantidadeResponsaveisSemApp = ObterResponsaveisSemApp(alunosUe, usuariosAlunosNotificacoesApp),
                                    QuantidadeAlunosSemLer = ObterAlunosSemLer(comunicado, alunosUe, usuariosAlunosNotificacoesApp),
                                    QuantidadeResponsaveisSemLer = ObterResponsaveisSemLer(comunicado, alunosUe, usuariosAlunosNotificacoesApp),
                                }
                            ).ToArray();

            var tudoConsolidado =
                smeConsolidado
                .Union(dreConsolidado)
                .Union(ueConsolidado)
                .ToArray();

            return tudoConsolidado;
        }

        private long ObterResponsaveisSemLer(ComunicadoSgpDto comunicado, IEnumerable<ResponsavelAlunoEOLDto> alunosComunicado, IEnumerable<UsuarioAlunoNotificacaoApp> usuariosAlunosNotificacoesApp)
        {
            var valido = alunosComunicado
                .AsParallel()
                .Where(aluno =>
                    usuariosAlunosNotificacoesApp
                    .Any(uan => uan.CpfResponsavel == aluno.CpfResponsavel)
                )
                .Select(aluno => aluno.CpfResponsavel)
                .Distinct()
                .Count();

            var leu = usuariosAlunosNotificacoesApp
                .AsParallel()
                .Where(uan=>
                    uan.AnoLetivo == comunicado.AnoLetivo
                &&  uan.NotificacaoId == comunicado.Id
                &&  uan.CodigoAluno == comunicado.AlunoCodigo
                )
                .Select(aluno => aluno.CpfResponsavel)
                .Distinct()
                .Count();

            var qtd = valido - leu;
            return qtd;
        }

        private long ObterAlunosSemLer(ComunicadoSgpDto comunicado, IEnumerable<ResponsavelAlunoEOLDto> alunosComunicado, IEnumerable<UsuarioAlunoNotificacaoApp> usuariosAlunosNotificacoesApp)
        {
            var valido = alunosComunicado
                .AsParallel()
                .Where(aluno =>
                    usuariosAlunosNotificacoesApp
                    .Any(uan => uan.CpfResponsavel == aluno.CpfResponsavel)
                )
                .Select(aluno => aluno.CodigoAluno)
                .Distinct()
                .Count();

            var leu = usuariosAlunosNotificacoesApp
                .AsParallel()
                .Where(uan =>
                    uan.AnoLetivo == comunicado.AnoLetivo
                 && uan.NotificacaoId == comunicado.Id
                 && uan.CodigoAluno == comunicado.AlunoCodigo
                )
                .Select(aluno => aluno.CodigoAluno)
                .Distinct()
                .Count();

            var qtd = valido - leu;
            return qtd;
        }

        private long ObterResponsaveisSemApp(IEnumerable<ResponsavelAlunoEOLDto> alunosComunicado, IEnumerable<UsuarioAlunoNotificacaoApp> usuariosAlunosNotificacoesApp)
        {
            var valido = alunosComunicado
                .AsParallel()
                .Where(aluno =>
                    usuariosAlunosNotificacoesApp
                    .Any(uan => uan.CpfResponsavel == aluno.CpfResponsavel)
                )
                .Select(aluno => aluno.CpfResponsavel)
                .Distinct()
                .Count();

            var total = alunosComunicado
                .AsParallel()
                .Select(aluno => aluno.CpfResponsavel)
                .Distinct()
                .Count();

            var qtd = total - valido;
            return qtd;
        }

        private long ObterAlunosSemApp(IEnumerable<ResponsavelAlunoEOLDto> alunosComunicado, IEnumerable<UsuarioAlunoNotificacaoApp> usuariosAlunosNotificacoesApp)
        {
            var valido = alunosComunicado
                .AsParallel()
                .Where(aluno =>
                    usuariosAlunosNotificacoesApp
                    .Any(uan => uan.CpfResponsavel == aluno.CpfResponsavel)
                )
                .Select(aluno => aluno.CodigoAluno)
                .Distinct()
                .Count();

            var total = alunosComunicado
                .AsParallel()
                .Select(aluno => aluno.CodigoAluno)
                .Distinct()
                .Count();

            var qtd = total - valido;
            return qtd;
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
