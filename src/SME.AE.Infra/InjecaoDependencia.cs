using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Interfaces.Geral;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Infra.Autenticacao;
using SME.AE.Infra.Persistencia;
using SME.AE.Infra.Persistencia.Cache;
using SME.AE.Infra.Persistencia.Repositorios;
using SME.AE.Infra.Persistencia.Repositorios.Externos;
using SME.AE.Infra.Persistencia.Repositorios.Externos.CoreSSO;

namespace SME.AE.Infra
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddDbContext<AplicacaoContext>(options =>
                options.UseNpgsql(
                    ConnectionStrings.Conexao,
                    b => b.MigrationsAssembly(typeof(AplicacaoContext).Assembly.FullName)));

            services.AddTransient(typeof(IConnectionMultiplexerAe), typeof(ConnectionMultiplexerAe));
            services.AddTransient(typeof(IAplicacaoContext), typeof(AplicacaoContext));
            services.AddTransient(typeof(IUsuarioRepository), typeof(UsuarioRepository));
            services.AddTransient(typeof(IAutenticacaoRepositorio), typeof(AutenticacaoRepositorio));
            services.AddTransient(typeof(INotificacaoRepository), typeof(NotificacaoRepository));
            services.AddTransient(typeof(IAlunoRepositorio), typeof(AlunoRepositorio));
            services.AddTransient(typeof(IGrupoComunicadoRepository), typeof(GrupoComunicadoRepository));
            services.AddTransient(typeof(IUsuarioNotificacaoRepositorio), typeof(UsuarioNotificacaoRepositorio));
            services.AddTransient(typeof(IUsuarioCoreSSORepositorio), typeof(UsuarioCoreSSORepositorio));
            services.AddTransient(typeof(ITesteRepositorio), typeof(TesteRepositorio));
            services.AddTransient(typeof(IUsuarioGrupoRepositorio), typeof(UsuarioGrupoRepositorio));
            services.AddTransient(typeof(IUsuarioSenhaHistoricoCoreSSORepositorio), typeof(UsuarioSenhaHistoricoCoreSSORepositorio));
            services.AddTransient(typeof(IConfiguracaoEmailRepositorio), typeof(ConfiguracaoEmailRepositorio));
            services.AddTransient(typeof(ITermosDeUsoRepositorio), typeof(TermosDeUsoRepositorio));
            services.AddTransient(typeof(IAceiteTermosDeUsoRepositorio), typeof(AceiteTermosDeUsoRepositorio));
            services.AddTransient(typeof(ICacheRepositorio), typeof(CacheRepositorio));

            services.AddTransient<IParametrosEscolaAquiRepositorio, ParametroEscolaAquiRepositorio>();
            services.AddTransient<IEventoRepositorio, EventoRepositorio>();
            services.AddTransient<IEventoSgpRepositorio, EventoSgpRepositorio>();

            services.AddDefaultIdentity<UsuarioAplicacao>().AddEntityFrameworkStores<AplicacaoContext>();

            services.AddIdentityServer().AddApiAuthorization<UsuarioAplicacao, AplicacaoContext>();
            services.AddTransient<IAutenticacaoService, AutenticacaoService>();
            services.AddAuthentication().AddIdentityServerJwt();
            return services;
        }
    }
}
