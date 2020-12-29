
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ConsolidarAdesaoEOLCasoDeUso
    {
        private readonly IResponsavelEOLRepositorio responsavelEOLRepositorio;
        private readonly IDashboardAdesaoRepositorio dashboardAdesaoRepositorio;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IDreSgpRepositorio dreSgpRepositorio;
        private readonly IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio;

        public ConsolidarAdesaoEOLCasoDeUso(IResponsavelEOLRepositorio responsavelEOLRepositorio,
                                            IDashboardAdesaoRepositorio dashboardAdesaoRepositorio,
                                            IUsuarioRepository usuarioRepository,
                                            IDreSgpRepositorio dreSgpRepositorio,
                                            IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio)
        {
            this.responsavelEOLRepositorio = responsavelEOLRepositorio ?? throw new System.ArgumentNullException(nameof(responsavelEOLRepositorio));
            this.dashboardAdesaoRepositorio = dashboardAdesaoRepositorio ?? throw new ArgumentNullException(nameof(dashboardAdesaoRepositorio));
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            this.dreSgpRepositorio = dreSgpRepositorio ?? throw new ArgumentNullException(nameof(dreSgpRepositorio));
            this.workerProcessoAtualizacaoRepositorio = workerProcessoAtualizacaoRepositorio ?? throw new ArgumentNullException(nameof(workerProcessoAtualizacaoRepositorio));
        }

        public async Task ExecutarAsync()
        {
            var usuariosDoSistemaEA = await ObterUsuariosEscolaAqui();
            var adesaoConsolidada = await ObterAdesaoConsolidada(usuariosDoSistemaEA);
            await dashboardAdesaoRepositorio.IncluiOuAtualizaPorDreUeTurmaEmBatch(adesaoConsolidada);
            await workerProcessoAtualizacaoRepositorio.IncluiOuAtualizaUltimaAtualizacao("ConsolidarAdesaoEOL");
        }

        private async Task<IEnumerable<Dominio.Entidades.Usuario>> ObterUsuariosEscolaAqui()
        {
            return await usuarioRepository.ObterTodosUsuariosAtivos();
        }

        private async Task<IEnumerable<DashboardAdesaoDto>> ObterAdesaoConsolidada(IEnumerable<Dominio.Entidades.Usuario> usuariosDoSistema)
        {
            var dresDoSistema = await dreSgpRepositorio.ObterTodosCodigoDresAtivasAsync();

            var listaDashBoardsParaIncluir = new List<DashboardAdesaoDto>();

            foreach (var dreCodigo in dresDoSistema)
            {
                var responsaveisDreEOL =
                    (await responsavelEOLRepositorio.ListarCpfResponsavelDaDreUeTurma(dreCodigo))
                    .ToList();

                listaDashBoardsParaIncluir = TrataTurmas(usuariosDoSistema, responsaveisDreEOL, listaDashBoardsParaIncluir);

                listaDashBoardsParaIncluir = TrataUes(responsaveisDreEOL, listaDashBoardsParaIncluir);

                listaDashBoardsParaIncluir = TrataDre(listaDashBoardsParaIncluir, dreCodigo);
            }

            var registrosDRES = listaDashBoardsParaIncluir.Where(a => a.codigo_turma == 0 && string.IsNullOrEmpty(a.ue_codigo)).ToList();


            var registroSMEParaIncluir = new DashboardAdesaoDto()
            {
                codigo_turma = 0,
                dre_codigo = string.Empty,
                dre_nome = string.Empty,
                ue_codigo = string.Empty,
                ue_nome = string.Empty,
                usuarios_cpf_invalidos = registrosDRES.Sum(a => a.usuarios_cpf_invalidos),
                usuarios_primeiro_acesso_incompleto = registrosDRES.Sum(a => a.usuarios_primeiro_acesso_incompleto),
                usuarios_sem_app_instalado = registrosDRES.Sum(a => a.usuarios_sem_app_instalado),
                usuarios_validos = registrosDRES.Sum(a => a.usuarios_validos)
            };

            listaDashBoardsParaIncluir.Add(registroSMEParaIncluir);

            return listaDashBoardsParaIncluir;

        }

        private List<DashboardAdesaoDto> TrataDre(List<DashboardAdesaoDto> listaDashBoardDresParaIncluir, long dreCodigo)
        {
            var registroParaTratarDre = listaDashBoardDresParaIncluir.FirstOrDefault();


            var registrosUes = listaDashBoardDresParaIncluir.Where(a => a.codigo_turma == 0 && a.dre_codigo == dreCodigo.ToString());

            var registroDreParaIncluir = new DashboardAdesaoDto()
            {
                codigo_turma = 0,
                ue_codigo = string.Empty,
                ue_nome = string.Empty,
                dre_codigo = registroParaTratarDre.dre_codigo,
                dre_nome = registroParaTratarDre.dre_nome,
                usuarios_cpf_invalidos = registrosUes.Sum(a => a.usuarios_cpf_invalidos),
                usuarios_primeiro_acesso_incompleto = registrosUes.Sum(a => a.usuarios_primeiro_acesso_incompleto),
                usuarios_sem_app_instalado = registrosUes.Sum(a => a.usuarios_sem_app_instalado),
                usuarios_validos = registrosUes.Sum(a => a.usuarios_validos)
            };

            listaDashBoardDresParaIncluir.Add(registroDreParaIncluir);

            return listaDashBoardDresParaIncluir;
        }

        private List<DashboardAdesaoDto> TrataUes(List<ResponsavelEOLDto> responsaveisDreEOL, List<DashboardAdesaoDto> listaDashBoardsParaIncluir)
        {
            var listaUesParaTratar = responsaveisDreEOL.Select(a => a.CodigoUe).Distinct();

            foreach (var ueParaTratar in listaUesParaTratar)
            {
                var registrosDaUe = listaDashBoardsParaIncluir.Where(a => a.ue_codigo == ueParaTratar);

                var registroParaTratarUe = registrosDaUe.FirstOrDefault();

                var registroDashboardDaUe = new DashboardAdesaoDto()
                {
                    codigo_turma = 0,
                    dre_codigo = registroParaTratarUe.dre_codigo,
                    dre_nome = registroParaTratarUe.dre_nome,
                    ue_codigo = registroParaTratarUe.ue_codigo,
                    ue_nome = registroParaTratarUe.ue_nome,
                    usuarios_cpf_invalidos = registrosDaUe.Sum(a => a.usuarios_cpf_invalidos),
                    usuarios_primeiro_acesso_incompleto = registrosDaUe.Sum(a => a.usuarios_primeiro_acesso_incompleto),
                    usuarios_sem_app_instalado = registrosDaUe.Sum(a => a.usuarios_sem_app_instalado),
                    usuarios_validos = registrosDaUe.Sum(a => a.usuarios_validos)
                };

                listaDashBoardsParaIncluir.Add(registroDashboardDaUe);
            }

            return listaDashBoardsParaIncluir;
        }

        private List<DashboardAdesaoDto> TrataTurmas(IEnumerable<Dominio.Entidades.Usuario> usuariosDoSistema, List<ResponsavelEOLDto> responsaveisDreEOL, List<DashboardAdesaoDto> listaDashBoardsParaIncluir)
        {
            var listaTurmasParaTratar = responsaveisDreEOL.Select(a => a.CodigoTurma).Distinct();

            //Turmas da Dre
            foreach (var turmaParaTratar in listaTurmasParaTratar)
            {
                var usuariosDaTurma = responsaveisDreEOL.Where(a => a.CodigoTurma == turmaParaTratar).ToList();

                var listaTodosDaTurmaParaTratar = new List<DashboardAdesaoDto>();

                foreach (var usuarioDaTurma in usuariosDaTurma)
                {
                    var dashBoardParaAdicionar = ProcessaResponsavel(usuarioDaTurma, usuariosDoSistema);
                    listaTodosDaTurmaParaTratar.Add(dashBoardParaAdicionar);
                }

                var registroParaObterDados = usuariosDaTurma.FirstOrDefault();


                var registroDashBoardTurmaAgrupado = new DashboardAdesaoDto()
                {
                    codigo_turma = turmaParaTratar,
                    dre_codigo = registroParaObterDados.CodigoDre,
                    dre_nome = registroParaObterDados.Dre,
                    ue_codigo = registroParaObterDados.CodigoUe,
                    ue_nome = registroParaObterDados.Ue,
                    usuarios_cpf_invalidos = listaTodosDaTurmaParaTratar.Sum(a => a.usuarios_cpf_invalidos),
                    usuarios_primeiro_acesso_incompleto = listaTodosDaTurmaParaTratar.Sum(a => a.usuarios_primeiro_acesso_incompleto),
                    usuarios_sem_app_instalado = listaTodosDaTurmaParaTratar.Sum(a => a.usuarios_sem_app_instalado),
                    usuarios_validos = listaTodosDaTurmaParaTratar.Sum(a => a.usuarios_validos)
                };

                listaDashBoardsParaIncluir.Add(registroDashBoardTurmaAgrupado);
            }

            return listaDashBoardsParaIncluir;
        }

        private DashboardAdesaoDto ProcessaResponsavel(ResponsavelEOLDto responsavel, IEnumerable<Dominio.Entidades.Usuario> usuariosDoSistema)
        {
            var cpf = responsavel.CpfResponsavel.ToString("00000000000");
            var cpfValido = ValidacaoCpf.Valida(cpf);

            var usuarios_primeiro_acesso_incompleto = 0;
            var usuarios_validos = 0;
            var usuarios_cpf_invalidos = 0;
            var usuarios_sem_app_instalado = 0;

            if (cpfValido)
            {
                var usuarioDoSistema = usuariosDoSistema.FirstOrDefault(a => a.Cpf == cpf);

                if (usuarioDoSistema == null)
                {
                    usuarios_sem_app_instalado = 1;
                }
                else
                {
                    usuarios_primeiro_acesso_incompleto = usuarioDoSistema.PrimeiroAcesso ? 1 : 0;
                    usuarios_validos = !usuarioDoSistema.PrimeiroAcesso ? 1 : 0;
                }
            }
            else
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
