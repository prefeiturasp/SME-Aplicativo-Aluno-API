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
            var consolidacaoNotificacoes = ConsolidarComunicadosUsuariosAlunos(usuariosAlunos, comunicadosAtivos);
            await consolidarLeituraNotificacaoRepositorio.SalvarConsolidacaoNotificacoesEmBatch(consolidacaoNotificacoes);
            await workerProcessoAtualizacaoRepositorio.IncluiOuAtualizaUltimaAtualizacao("ConsolidarLeituraNotificacao");
        }

        private IEnumerable<ConsolidacaoNotificacaoDto> ConsolidarComunicadosUsuariosAlunos(IEnumerable<ResponsavelAlunoEOLDto> usuariosAlunos, IEnumerable<ComunicadoSgpDto> comunicadosAtivos)
        {
            var consolidado = comunicadosAtivos
                .SelectMany(comunicado => ConsolidarComunicado(comunicado, usuariosAlunos))
                .ToArray();
            return consolidado;
        }

        private IEnumerable<ConsolidacaoNotificacaoDto> ConsolidarComunicado(ComunicadoSgpDto comunicado, IEnumerable<ResponsavelAlunoEOLDto> usuariosAlunos)
        {
            var alunosComunicado = 
                ObterAlunosDoComunicado(comunicado, usuariosAlunos)
                .Distinct()
                .ToArray();

            var smeConsolidado = new ConsolidacaoNotificacaoDto[] { new ConsolidacaoNotificacaoDto {
                NotificacaoId = comunicado.Id,
                AnoLetivo = comunicado.AnoLetivo,
                DreCodigo = "",
                UeCodigo = "",
                QuantidadeAlunos =
                    alunosComunicado
                        .Select(aluno => aluno.CodigoAluno)
                        .Distinct()
                        .Count(),
                QuantidadeResponsaveis =
                    alunosComunicado
                        .Select(aluno => aluno.CpfResponsavel)
                        .Distinct()
                        .Count()
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
                                    QuantidadeAlunos =
                                        alunosDre
                                            .Select(aluno => aluno.CodigoAluno)
                                            .Distinct()
                                            .Count(),
                                    QuantidadeResponsaveis =
                                        alunosDre
                                            .Select(aluno => aluno.CpfResponsavel)
                                            .Distinct()
                                            .Count(),
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
                                    QuantidadeAlunos =
                                        alunosUe
                                            .Select(aluno => aluno.CodigoAluno)
                                            .Distinct()
                                            .Count(),
                                    QuantidadeResponsaveis =
                                        alunosUe
                                            .Select(aluno => aluno.CpfResponsavel)
                                            .Distinct()
                                            .Count(),
                                }
                            ).ToArray();
            var tudoConsolidado =
                smeConsolidado
                .Union(dreConsolidado)
                .Union(ueConsolidado)
                .ToArray();

            return tudoConsolidado;
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
