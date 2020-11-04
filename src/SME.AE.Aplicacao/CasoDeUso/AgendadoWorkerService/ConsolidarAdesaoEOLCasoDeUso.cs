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
    public class ConsolidarAdesaoEOLCasoDeUso
    {
        private readonly IResponsavelEOLRepositorio responsavelEOLRepositorio;
        private readonly IDashboardAdesaoRepositorio dashboardAdesaoRepositorio;
        private readonly IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio;
        private readonly IUsuarioRepository usuarioRepository;


        private Dictionary<string, Dominio.Entidades.Usuario> UsuariosEscolaAquiDict = new Dictionary<string, Dominio.Entidades.Usuario>();

        public ConsolidarAdesaoEOLCasoDeUso(IResponsavelEOLRepositorio responsavelEOLRepositorio,
                                            IDashboardAdesaoRepositorio dashboardAdesaoRepositorio,
                                            IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio,
                                            IUsuarioRepository usuarioRepository)
        {
            this.responsavelEOLRepositorio = responsavelEOLRepositorio ?? throw new System.ArgumentNullException(nameof(responsavelEOLRepositorio));
            this.dashboardAdesaoRepositorio = dashboardAdesaoRepositorio ?? throw new ArgumentNullException(nameof(dashboardAdesaoRepositorio));
            this.workerProcessoAtualizacaoRepositorio = workerProcessoAtualizacaoRepositorio ?? throw new ArgumentNullException(nameof(workerProcessoAtualizacaoRepositorio));
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task ExecutarAsync()
        {
            await ObterUsuariosEscolaAqui();
            var adesaoConsolidada = await ObterAdesaoConsolidada();
            await dashboardAdesaoRepositorio.IncluiOuAtualizaPorDreUeTurmaEmBatch(adesaoConsolidada);
            await workerProcessoAtualizacaoRepositorio.IncluiOuAtualizaUltimaAtualizacao("ConsolidarAdesaoEOL");
        }

        private async Task ObterUsuariosEscolaAqui()
        {
            UsuariosEscolaAquiDict.Clear();
            var usuariosEscolaAqui = await usuarioRepository.ListarAsync();
            usuariosEscolaAqui.ForEach(usuario => 
            { 
                UsuariosEscolaAquiDict.Add(usuario.Cpf, usuario); 
            });
        }

        private async Task<IEnumerable<DashboardAdesaoDto>> ObterAdesaoConsolidada()
        {
            var responsaveisEOL =
                (await responsavelEOLRepositorio.ListarCpfResponsavelDaDreUeTurma())
                .AsParallel();

            var porSME =
                responsaveisEOL
                .GroupBy(
                        responsavelChave => responsavelChave.CpfResponsavel,
                        responsavelValor => responsavelValor,
                        (chave, valor) => ProcessaResponsavel(valor.First())
                    )
                .GroupBy(
                        todosChave => 0,
                        todosValor => todosValor,
                        (chave, valor) => new DashboardAdesaoDto
                        {
                            dre_codigo = "",
                            dre_nome = "",
                            ue_codigo = "",
                            ue_nome = "",
                            codigo_turma = 0,
                            usuarios_validos = valor.Sum(adesao => adesao.usuarios_validos),
                            usuarios_cpf_invalidos = valor.Sum(adesao => adesao.usuarios_cpf_invalidos),
                            usuarios_primeiro_acesso_incompleto = valor.Sum(adesao => adesao.usuarios_primeiro_acesso_incompleto),
                            usuarios_sem_app_instalado = valor.Sum(adesao => adesao.usuarios_sem_app_instalado),
                        }
                    );

            var porDRE =
                responsaveisEOL
                .GroupBy(
                        responsavelChave => responsavelChave.CodigoDre,
                        responsavelValor => responsavelValor,
                        (chave, valor) => 
                            valor
                            .GroupBy(
                                    responsavelChave => responsavelChave.CpfResponsavel,
                                    responsavelValor => responsavelValor,
                                    (chave, valor) => ProcessaResponsavel(valor.First())
                                )
                    )
                .SelectMany(resp => resp)
                .GroupBy(
                        todosChave => todosChave.dre_codigo,
                        todosValor => todosValor,
                        (chave, valor) => new DashboardAdesaoDto
                        {
                            dre_codigo = valor.First().dre_codigo,
                            dre_nome = valor.First().dre_nome,
                            ue_codigo = "",
                            ue_nome = "",
                            codigo_turma = 0,
                            usuarios_validos = valor.Sum(adesao => adesao.usuarios_validos),
                            usuarios_cpf_invalidos = valor.Sum(adesao => adesao.usuarios_cpf_invalidos),
                            usuarios_primeiro_acesso_incompleto = valor.Sum(adesao => adesao.usuarios_primeiro_acesso_incompleto),
                            usuarios_sem_app_instalado = valor.Sum(adesao => adesao.usuarios_sem_app_instalado),
                        }
                    );

            var porUE =
                responsaveisEOL
                .GroupBy(
                        responsavelChave => responsavelChave.CodigoUe,
                        responsavelValor => responsavelValor,
                        (chave, valor) =>
                            valor
                            .GroupBy(
                                    responsavelChave => responsavelChave.CpfResponsavel,
                                    responsavelValor => responsavelValor,
                                    (chave, valor) => ProcessaResponsavel(valor.First())
                                )
                    )
                .SelectMany(resp => resp)
                .GroupBy(
                        todosChave => todosChave.ue_codigo,
                        todosValor => todosValor,
                        (chave, valor) => new DashboardAdesaoDto
                        {
                            dre_codigo = valor.First().dre_codigo,
                            dre_nome = valor.First().dre_nome,
                            ue_codigo = valor.First().ue_codigo,
                            ue_nome = valor.First().ue_nome,
                            codigo_turma = 0,
                            usuarios_validos = valor.Sum(adesao => adesao.usuarios_validos),
                            usuarios_cpf_invalidos = valor.Sum(adesao => adesao.usuarios_cpf_invalidos),
                            usuarios_primeiro_acesso_incompleto = valor.Sum(adesao => adesao.usuarios_primeiro_acesso_incompleto),
                            usuarios_sem_app_instalado = valor.Sum(adesao => adesao.usuarios_sem_app_instalado),
                        }
                    );

            var porTurma =
                responsaveisEOL
                .GroupBy(
                        responsavelChave => responsavelChave.CodigoTurma,
                        responsavelValor => responsavelValor,
                        (chave, valor) =>
                            valor
                            .GroupBy(
                                    responsavelChave => responsavelChave.CpfResponsavel,
                                    responsavelValor => responsavelValor,
                                    (chave, valor) => ProcessaResponsavel(valor.First())
                                )
                    )
                .SelectMany(resp => resp)
                .GroupBy(
                        todosChave => todosChave.codigo_turma,
                        todosValor => todosValor,
                        (chave, valor) => new DashboardAdesaoDto
                        {
                            dre_codigo = valor.First().dre_codigo,
                            dre_nome = valor.First().dre_nome,
                            ue_codigo = valor.First().ue_codigo,
                            ue_nome = valor.First().ue_nome,
                            codigo_turma = 0,
                            usuarios_validos = valor.Sum(adesao => adesao.usuarios_validos),
                            usuarios_cpf_invalidos = valor.Sum(adesao => adesao.usuarios_cpf_invalidos),
                            usuarios_primeiro_acesso_incompleto = valor.Sum(adesao => adesao.usuarios_primeiro_acesso_incompleto),
                            usuarios_sem_app_instalado = valor.Sum(adesao => adesao.usuarios_sem_app_instalado),
                        }
                    );

            var adesaoConsolidada =
                porSME
                .Union(porDRE)
                .Union(porUE)
                .Union(porTurma)
                ;
                
            return adesaoConsolidada;
        }

        private DashboardAdesaoDto ProcessaResponsavel(ResponsavelEOLDto responsavel)
        {
            var cpf = responsavel.CpfResponsavel.ToString("00000000000");
            var cpfValido = ValidacaoCpf.Valida(cpf);

            var usuarios_primeiro_acesso_incompleto = 0;
            var usuarios_validos = 0;
            var usuarios_cpf_invalidos = 0;
            var usuarios_sem_app_instalado = 0;

            if(cpfValido)
            {
                UsuariosEscolaAquiDict.TryGetValue(cpf, out var usuarioEscolaAqui);
                if(usuarioEscolaAqui == null || usuarioEscolaAqui.Excluido)
                {
                    usuarios_sem_app_instalado = 1;
                } else
                {
                    usuarios_primeiro_acesso_incompleto = usuarioEscolaAqui.PrimeiroAcesso ? 1 : 0;
                    usuarios_validos = !usuarioEscolaAqui.PrimeiroAcesso ? 1 : 0;
                }
            } else
            {
                usuarios_cpf_invalidos = 1;
            }

            var dashboard_adesao = new DashboardAdesaoDto
                {
                    dre_codigo = responsavel.CodigoDre,
                    dre_nome = responsavel.Dre,
                    ue_codigo = responsavel.CodigoUe,
                    ue_nome = responsavel.Ue,
                    codigo_turma = responsavel.CodigoTurma,
                    usuarios_primeiro_acesso_incompleto = usuarios_primeiro_acesso_incompleto,
                    usuarios_validos = usuarios_validos,
                    usuarios_cpf_invalidos = usuarios_cpf_invalidos,
                    usuarios_sem_app_instalado = usuarios_sem_app_instalado
                };
            return dashboard_adesao;
        }
    }
}
