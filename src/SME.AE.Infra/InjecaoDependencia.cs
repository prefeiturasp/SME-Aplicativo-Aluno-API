using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Interfaces.Geral;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Servicos;
using SME.AE.Comum;
using SME.AE.Infra.Autenticacao;
using SME.AE.Infra.Persistencia;
using SME.AE.Infra.Persistencia.Repositorios;
using SME.AE.Infra.Persistencia.Repositorios.Externos;
using SME.AE.Infra.Persistencia.Repositorios.Externos.CoreSSO;

namespace SME.AE.Infra
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, VariaveisGlobaisOptions variaveisGlobais)
        {
            services.AddDbContext<AplicacaoContext>(options =>
                options.UseNpgsql(
                    variaveisGlobais.AEConnection,
                    b => b.MigrationsAssembly(typeof(AplicacaoContext).Assembly.FullName)));

            services.TryAddScoped(typeof(IAplicacaoContext), typeof(AplicacaoContext));
            services.TryAddScoped(typeof(IUsuarioRepository), typeof(UsuarioRepository));
            services.TryAddScoped(typeof(IAutenticacaoRepositorio), typeof(AutenticacaoRepositorio));
            services.TryAddScoped(typeof(INotificacaoRepositorio), typeof(NotificacaoRepositorio));
            services.TryAddScoped(typeof(IAlunoRepositorio), typeof(AlunoRepositorio));
            services.TryAddScoped(typeof(IGrupoComunicadoRepository), typeof(GrupoComunicadoRepository));
            services.TryAddScoped(typeof(IUsuarioNotificacaoRepositorio), typeof(UsuarioNotificacaoRepositorio));
            services.TryAddScoped(typeof(INotificacaoTurmaRepositorio), typeof(NotificacaoTurmaRepositorio));
            services.TryAddScoped(typeof(INotificacaoAlunoRepositorio), typeof(NotificacaoAlunoRepositorio));
            services.TryAddScoped(typeof(IUsuarioCoreSSORepositorio), typeof(UsuarioCoreSSORepositorio));
            services.TryAddScoped(typeof(IUsuarioGrupoRepositorio), typeof(UsuarioGrupoRepositorio));
            services.TryAddScoped(typeof(IUsuarioSenhaHistoricoCoreSSORepositorio), typeof(UsuarioSenhaHistoricoCoreSSORepositorio));
            services.TryAddScoped(typeof(IConfiguracaoEmailRepositorio), typeof(ConfiguracaoEmailRepositorio));
            services.TryAddScoped(typeof(IResponsavelEOLRepositorio), typeof(ResponsavelEOLRepositorio));
            services.TryAddScoped(typeof(ITermosDeUsoRepositorio), typeof(TermosDeUsoRepositorio));
            services.TryAddScoped(typeof(IOutroServicoRepositorio), typeof(OutroServicoRepositorio));
            services.TryAddScoped(typeof(IAceiteTermosDeUsoRepositorio), typeof(AceiteTermosDeUsoRepositorio));
            services.TryAddScoped(typeof(IAdesaoRepositorio), typeof(AdesaoRepositorio));
            services.TryAddScoped(typeof(IParametrosEscolaAquiRepositorio), (typeof(ParametroEscolaAquiRepositorio)));
            services.TryAddScoped(typeof(IEventoRepositorio), (typeof(EventoRepositorio)));
            services.TryAddScoped(typeof(IEventoSgpRepositorio), (typeof(EventoSgpRepositorio)));
            services.TryAddScoped(typeof(IWorkerProcessoAtualizacaoRepositorio), typeof(WorkerProcessoAtualizacaoRepositorio));
            services.TryAddScoped(typeof(IFrequenciaAlunoRepositorio), typeof(FrequenciaAlunoRepositorio));
            services.TryAddScoped(typeof(INotaAlunoRepositorio), typeof(NotaAlunoRepositorio));
            services.TryAddScoped(typeof(ITurmaRepositorio), typeof(TurmaRepositorio));
            services.TryAddScoped(typeof(INotaAlunoCorRepositorio), typeof(NotaAlunoCorRepositorio));
            services.TryAddScoped(typeof(IDadosLeituraRepositorio), typeof(DadosLeituraRepositorio));
            services.TryAddScoped(typeof(IUnidadeEscolarRepositorio), typeof(UnidadeEscolarRepositorio));
            services.TryAddScoped(typeof(IDreSgpRepositorio), typeof(DreSgpRepositorio));
            services.TryAddScoped(typeof(IRemoverConexaoIdleRepository), typeof(RemoverConexaoIdleRepository));
            services.TryAddScoped(typeof(ICacheRepositorio), typeof(CacheRepositorio));
            services.TryAddScoped(typeof(INotificacaoRepositorio), typeof(NotificacaoRepositorio));
            services.TryAddScoped(typeof(IServicoLog), typeof(ServicoLog));
            services.TryAddScoped(typeof(INotaAlunoSgpRepositorio), typeof(NotaAlunoSgpRepositorio));
            services.TryAddScoped(typeof(IUeSgpRepositorio), typeof(UeSgpRepositorio));
            services.TryAddScoped(typeof(IFrequenciaAlunoSgpRepositorio), typeof(FrequenciaAlunoSgpRepositorio));
            services.TryAddScoped(typeof(IComponenteCurricularSgpRepositorio), typeof(ComponenteCurricularSgpRepositorio));
            services.AddDefaultIdentity<UsuarioAplicacao>().AddEntityFrameworkStores<AplicacaoContext>();
            services.AddIdentityServer().AddApiAuthorization<UsuarioAplicacao, AplicacaoContext>();
            services.TryAddScoped<IAutenticacaoService, AutenticacaoService>();
            services.AddAuthentication().AddIdentityServerJwt();
            return services;
        }
    }
}
