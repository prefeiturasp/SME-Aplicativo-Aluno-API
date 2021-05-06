using Sentry;
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
        private List<DashboardAdesaoUnificacaoDto> listaDeCpfsUtilizados { get; set; }
        private int cpfsInvalidosSME { get; set; }

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
            listaDeCpfsUtilizados = new List<DashboardAdesaoUnificacaoDto>();
            cpfsInvalidosSME = 0;
        }

        public async Task ExecutarAsync()
        {
            var usuariosDoSistemaEA = await ObterUsuariosEscolaAqui();
            await ObterAdesaoConsolidada(usuariosDoSistemaEA);
            await workerProcessoAtualizacaoRepositorio.IncluiOuAtualizaUltimaAtualizacao("ConsolidarAdesaoEOL");
        }

        private async Task<IEnumerable<Dominio.Entidades.Usuario>> ObterUsuariosEscolaAqui()
        {
            return await usuarioRepository.ObterTodosUsuariosAtivos();
        }

        private async Task ObterAdesaoConsolidada(IEnumerable<Dominio.Entidades.Usuario> usuariosDoSistema)
        {
            try
            {
                var anoLetivoAtual = DateTime.Now.Year;
                var dresDoSistema = await dreSgpRepositorio.ObterTodosCodigoDresAtivasAsync();

                foreach (var dreCodigo in dresDoSistema)
                {
                    var responsaveisDreEOL =
                        (await responsavelEOLRepositorio.ListarCpfResponsavelDaDreUeTurma(dreCodigo, anoLetivoAtual))
                        .ToList();

                    await TrataTurmas(usuariosDoSistema, responsaveisDreEOL);

                    await TrataUes(dreCodigo.ToString(), responsaveisDreEOL);

                    await TrataDre(dreCodigo.ToString(), responsaveisDreEOL);

                }

                await TrataSME();

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        private async Task TrataDre(string dreCodigo, List<ResponsavelEOLDto> responsaveis)
        {
            try
            {
                var listaDashBoardsParaIncluir = new List<DashboardAdesaoDto>();

                var registrosParaTratar = listaDeCpfsUtilizados.Where(a => a.DreCodigo == dreCodigo).ToList();

                var cpfsUnicosDaDre = registrosParaTratar.Where(a => a.CPF != 0).ToList().Select(a => a.CPF).Distinct();

                int cpfsInvalidos = 0, primeirosAcessos = 0, semAppInstalado = 0, validos = 0;

                cpfsUnicosDaDre.AsParallel()
                    .WithDegreeOfParallelism(6)
                    .ForAll(cpf =>
                    {
                        var registroParaSomar = registrosParaTratar.FirstOrDefault(a => a.CPF == cpf);
                        primeirosAcessos += registroParaSomar.PrimeiroAcessoIncompleto;
                        semAppInstalado += registroParaSomar.UsuarioSemAppInstalado;
                        validos += registroParaSomar.UsuarioValido;
                    });

                cpfsInvalidos = registrosParaTratar.Where(a => a.DreCodigo == dreCodigo && a.CPF == 0).ToList().Count();

                cpfsInvalidosSME += cpfsInvalidos;

                var registroParaTratarDre = responsaveis.FirstOrDefault(a => a.CodigoDre == dreCodigo);


                var registroDashboardDaDre = new DashboardAdesaoDto()
                {
                    codigo_turma = 0,
                    dre_codigo = registroParaTratarDre.CodigoDre,
                    dre_nome = registroParaTratarDre.Dre,
                    ue_codigo = string.Empty,
                    ue_nome = string.Empty,
                    usuarios_cpf_invalidos = cpfsInvalidos,
                    usuarios_primeiro_acesso_incompleto = primeirosAcessos,
                    usuarios_sem_app_instalado = semAppInstalado,
                    usuarios_validos = validos
                };

                listaDashBoardsParaIncluir.Add(registroDashboardDaDre);

                await dashboardAdesaoRepositorio.IncluiOuAtualizaPorDreUeTurmaEmBatch(listaDashBoardsParaIncluir);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        private async Task TrataSME()
        {
            try
            {
                var listaDashBoardsParaIncluir = new List<DashboardAdesaoDto>();

                var cpfsUnicosDaSME = listaDeCpfsUtilizados.Where(a => a.CPF != 0).Select(a => a.CPF).ToList().Distinct();

                int primeirosAcessos = 0, semAppInstalado = 0, validos = 0;

                cpfsUnicosDaSME.AsParallel()
                    .WithDegreeOfParallelism(8)
                    .ForAll(cpf =>
                    {
                        var registroParaSomar = listaDeCpfsUtilizados.FirstOrDefault(a => a.CPF == cpf);
                        primeirosAcessos += registroParaSomar.PrimeiroAcessoIncompleto;
                        semAppInstalado += registroParaSomar.UsuarioSemAppInstalado;
                        validos += registroParaSomar.UsuarioValido;
                    });

                var registroDashboardDaDre = new DashboardAdesaoDto()
                {
                    codigo_turma = 0,
                    dre_codigo = string.Empty,
                    dre_nome = string.Empty,
                    ue_codigo = string.Empty,
                    ue_nome = string.Empty,
                    usuarios_cpf_invalidos = cpfsInvalidosSME,
                    usuarios_primeiro_acesso_incompleto = primeirosAcessos,
                    usuarios_sem_app_instalado = semAppInstalado,
                    usuarios_validos = validos
                };

                listaDashBoardsParaIncluir.Add(registroDashboardDaDre);

                await dashboardAdesaoRepositorio.IncluiOuAtualizaPorDreUeTurmaEmBatch(listaDashBoardsParaIncluir);

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        private async Task TrataUes(string dreCodigo, List<ResponsavelEOLDto> responsaveis)
        {

            var listaDashBoardsParaIncluir = new List<DashboardAdesaoDto>();

            var registrosParaTratar = listaDeCpfsUtilizados.Where(a => a.DreCodigo == dreCodigo).ToList();

            var uesCodigosParaTratar = registrosParaTratar.Select(a => a.UeCodigo).Distinct().ToList();

            foreach (var ueParaTratar in uesCodigosParaTratar)
            {
                try
                {
                    var cpfValidosDaUe = registrosParaTratar.Where(a => a.UeCodigo == ueParaTratar && a.CPF != 0).ToList();

                    var cpfsUnicosDaUe = cpfValidosDaUe.Select(a => a.CPF).Distinct().ToList();

                    int cpfsInvalidos = 0, primeirosAcessos = 0, semAppInstalado = 0, validos = 0;

                    cpfsUnicosDaUe.AsParallel()
                        .WithDegreeOfParallelism(6)
                        .ForAll(cpf =>
                        {

                            var registroParaSomar = cpfValidosDaUe.FirstOrDefault(a => a.CPF == cpf);
                            primeirosAcessos += registroParaSomar.PrimeiroAcessoIncompleto;
                            semAppInstalado += registroParaSomar.UsuarioSemAppInstalado;
                            validos += registroParaSomar.UsuarioValido;
                        });


                    cpfsInvalidos = registrosParaTratar.Where(a => a.UeCodigo == ueParaTratar && a.CPF == 0).Count();

                    var registroParaTratarUe = responsaveis.FirstOrDefault(a => a.CodigoUe == ueParaTratar);


                    var registroDashboardDaUe = new DashboardAdesaoDto()
                    {
                        codigo_turma = 0,
                        dre_codigo = registroParaTratarUe.CodigoDre,
                        dre_nome = registroParaTratarUe.Dre,
                        ue_codigo = registroParaTratarUe.CodigoUe,
                        ue_nome = registroParaTratarUe.Ue,
                        usuarios_cpf_invalidos = cpfsInvalidos,
                        usuarios_primeiro_acesso_incompleto = primeirosAcessos,
                        usuarios_sem_app_instalado = semAppInstalado,
                        usuarios_validos = validos
                    };

                    listaDashBoardsParaIncluir.Add(registroDashboardDaUe);

                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                }
            }

            await dashboardAdesaoRepositorio.IncluiOuAtualizaPorDreUeTurmaEmBatch(listaDashBoardsParaIncluir);
        }

        private async Task TrataTurmas(IEnumerable<Dominio.Entidades.Usuario> usuariosDoSistema, List<ResponsavelEOLDto> responsaveisDreEOL)
        {

            var listaDashBoardsParaIncluir = new List<DashboardAdesaoDto>();

            var listaTurmasParaTratar = responsaveisDreEOL.Select(a => a.CodigoTurma).Distinct();
            try
            {
                //Turmas da Dre
                foreach (var turmaParaTratar in listaTurmasParaTratar)
                {
                    var usuariosDaTurma = responsaveisDreEOL.Where(a => a.CodigoTurma == turmaParaTratar).ToList();

                    var listaTodosDaTurmaParaTratar = new List<DashboardAdesaoDto>();

                    var cpfsParaTratar = listaDeCpfsUtilizados.Where(a => a.TurmaCodigo == turmaParaTratar).ToList();

                    foreach (var usuarioDaTurma in usuariosDaTurma)
                    {

                        var dashBoardParaAdicionar = ProcessaResponsavel(usuarioDaTurma, usuariosDoSistema, cpfsParaTratar);
                        if (dashBoardParaAdicionar != null)
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

                await dashboardAdesaoRepositorio.IncluiOuAtualizaPorDreUeTurmaEmBatch(listaDashBoardsParaIncluir);

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        private DashboardAdesaoDto ProcessaResponsavel(ResponsavelEOLDto responsavel, IEnumerable<Dominio.Entidades.Usuario> usuariosDoSistema, List<DashboardAdesaoUnificacaoDto> listaCpfsUnificados)
        {

            if (responsavel.CpfResponsavel == 0 || !listaCpfsUnificados.Any(a => a.CPF == responsavel.CpfResponsavel))
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

                listaDeCpfsUtilizados.Add(new DashboardAdesaoUnificacaoDto()
                {
                    CPF = long.Parse(cpf),
                    DreCodigo = responsavel.CodigoDre,
                    PrimeiroAcessoIncompleto = usuarios_primeiro_acesso_incompleto,
                    TurmaCodigo = responsavel.CodigoTurma,
                    UeCodigo = responsavel.CodigoUe,
                    UsuarioCpfInvalido = usuarios_cpf_invalidos,
                    UsuarioSemAppInstalado = usuarios_sem_app_instalado,
                    UsuarioValido = usuarios_validos
                });

                return dashboard_adesao;
            }
            return null;
        }
    }
}