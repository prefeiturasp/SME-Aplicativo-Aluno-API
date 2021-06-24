using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddScoped(typeof(IAplicacaoContext), typeof(AplicacaoContext));
            services.AddScoped(typeof(IUsuarioRepository), typeof(UsuarioRepository));
            services.AddScoped(typeof(IAutenticacaoRepositorio), typeof(AutenticacaoRepositorio));
            services.AddScoped(typeof(INotificacaoRepository), typeof(NotificacaoRepository));
            services.AddScoped(typeof(IAlunoRepositorio), typeof(AlunoRepositorio));
            services.AddScoped(typeof(IGrupoComunicadoRepository), typeof(GrupoComunicadoRepository));
            services.AddScoped(typeof(IUsuarioNotificacaoRepositorio), typeof(UsuarioNotificacaoRepositorio));
            services.AddScoped(typeof(IUsuarioCoreSSORepositorio), typeof(UsuarioCoreSSORepositorio));            
            services.AddScoped(typeof(IUsuarioGrupoRepositorio), typeof(UsuarioGrupoRepositorio));
            services.AddScoped(typeof(IUsuarioSenhaHistoricoCoreSSORepositorio), typeof(UsuarioSenhaHistoricoCoreSSORepositorio));
            services.AddScoped(typeof(IConfiguracaoEmailRepositorio), typeof(ConfiguracaoEmailRepositorio));
            services.AddScoped(typeof(IResponsavelEOLRepositorio), typeof(ResponsavelEOLRepositorio));
            services.AddScoped(typeof(ITermosDeUsoRepositorio), typeof(TermosDeUsoRepositorio));
            services.AddScoped(typeof(IAceiteTermosDeUsoRepositorio), typeof(AceiteTermosDeUsoRepositorio));
            services.AddScoped(typeof(IAdesaoRepositorio), typeof(AdesaoRepositorio));
            services.AddScoped(typeof(IParametrosEscolaAquiRepositorio), (typeof(ParametroEscolaAquiRepositorio)));
            services.AddScoped(typeof(IEventoRepositorio), (typeof(EventoRepositorio)));
            services.AddScoped(typeof(IEventoSgpRepositorio), (typeof(EventoSgpRepositorio)));
            services.AddScoped(typeof(IWorkerProcessoAtualizacaoRepositorio), typeof(WorkerProcessoAtualizacaoRepositorio));
            services.AddScoped(typeof(IFrequenciaAlunoRepositorio), typeof(FrequenciaAlunoRepositorio));
            services.AddScoped(typeof(INotaAlunoRepositorio), typeof(NotaAlunoRepositorio));
            services.AddScoped(typeof(ITurmaRepositorio), typeof(TurmaRepositorio));
            services.AddScoped(typeof(INotaAlunoCorRepositorio), typeof(NotaAlunoCorRepositorio));
            services.AddScoped(typeof(IDadosLeituraRepositorio), typeof(DadosLeituraRepositorio));
            services.AddScoped(typeof(IUnidadeEscolarRepositorio), typeof(UnidadeEscolarRepositorio));
            services.AddScoped(typeof(IDreSgpRepositorio), typeof(DreSgpRepositorio));
            services.AddScoped(typeof(IRemoverConexaoIdleRepository), typeof(RemoverConexaoIdleRepository));
            services.AddScoped(typeof(ICacheRepositorio), typeof(CacheRepositorio));
            services.AddScoped(typeof(IServicoLog), typeof(ServicoLog));
            services.AddDefaultIdentity<UsuarioAplicacao>().AddEntityFrameworkStores<AplicacaoContext>();
            services.AddIdentityServer().AddApiAuthorization<UsuarioAplicacao, AplicacaoContext>();
            services.AddScoped<IAutenticacaoService, AutenticacaoService>();
            services.AddAuthentication().AddIdentityServerJwt();
            return services;
        }
    }
}
