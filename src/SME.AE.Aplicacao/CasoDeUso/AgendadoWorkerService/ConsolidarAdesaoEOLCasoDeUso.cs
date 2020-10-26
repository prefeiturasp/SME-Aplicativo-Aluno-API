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
    public class ConsolidarAdesaoEOLCasoDeUso
    {
        private readonly IResponsavelEOLRepositorio responsavelEOLRepositorio;
        private readonly IDashboardAdesaoRepositorio dashboardAdesaoRepositorio;
        private readonly IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio;

        public ConsolidarAdesaoEOLCasoDeUso(IResponsavelEOLRepositorio responsavelEOLRepositorio,
                                            IDashboardAdesaoRepositorio dashboardAdesaoRepositorio,
                                            IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio)
        {
            this.responsavelEOLRepositorio = responsavelEOLRepositorio ?? throw new System.ArgumentNullException(nameof(responsavelEOLRepositorio));
            this.dashboardAdesaoRepositorio = dashboardAdesaoRepositorio ?? throw new ArgumentNullException(nameof(dashboardAdesaoRepositorio));
            this.workerProcessoAtualizacaoRepositorio = workerProcessoAtualizacaoRepositorio ?? throw new ArgumentNullException(nameof(workerProcessoAtualizacaoRepositorio));
        }

        public async Task ExecutarAsync()
        {
            var adesaoConsolidada = await ObterAdesaoConsolidada();
            await dashboardAdesaoRepositorio.IncluiOuAtualizaPorDreUeTurmaEmBatch(adesaoConsolidada);
            await workerProcessoAtualizacaoRepositorio.IncluiOuAtualizaUltimaAtualizacao("ConsolidarAdesaoEOL");
        }

        private async Task<IEnumerable<DashboardAdesaoDto>> ObterAdesaoConsolidada()
            => (await responsavelEOLRepositorio.ListarCpfResponsavelDaDreUeTurma())
            .AsParallel()
            .Select(r => ProcessaResponsavel(r))
            .GroupBy(
                a => new { a.dre_codigo, a.ue_codigo, a.codigo_turma },
                a => a,
                (key, value) => new DashboardAdesaoDto
                {
                    dre_codigo = key.dre_codigo,
                    ue_codigo = key.ue_codigo,
                    codigo_turma = key.codigo_turma,
                    dre_nome = value.FirstOrDefault().dre_nome,
                    ue_nome = value.FirstOrDefault().ue_nome,
                    usuarios_validos = value.Sum(adesao => adesao.usuarios_validos),
                    usuarios_cpf_invalidos = value.Sum(adesao => adesao.usuarios_cpf_invalidos),
                    usuarios_primeiro_acesso_incompleto = value.Sum(adesao => adesao.usuarios_primeiro_acesso_incompleto),
                    usuarios_sem_app_instalado = value.Sum(adesao => adesao.usuarios_sem_app_instalado),
                });
        
        private DashboardAdesaoDto ProcessaResponsavel(ResponsavelEOLDto responsavel)
        {
            var cpf = responsavel.CpfResponsavel.ToString("00000000000");
            var cpfValido = ValidacaoCpf.Valida(cpf);
            var dashboard_adesao = new DashboardAdesaoDto
                {
                    dre_codigo = responsavel.CodigoDre,
                    dre_nome = responsavel.Dre,
                    ue_codigo = responsavel.CodigoUe,
                    ue_nome = responsavel.Ue,
                    codigo_turma = responsavel.CodigoTurma,
                    usuarios_primeiro_acesso_incompleto = 0,
                    usuarios_validos = cpfValido ? 1 : 0,
                    usuarios_cpf_invalidos = cpfValido ? 0 : 1,
                    usuarios_sem_app_instalado = 0
                };
            return dashboard_adesao;
        }
    }
}
