using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Security.Claims;
using Dapper.FluentMap;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Interfaces.Geral;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Infra.Autenticacao;
using SME.AE.Infra.Persistencia;
using SME.AE.Infra.Persistencia.Repositorios;

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

            services.AddTransient(typeof(IAplicacaoContext), typeof(AplicacaoContext));
            services.AddTransient(typeof(IUsuarioRepository), typeof(UsuarioRepository));
            services.AddTransient(typeof(IAutenticacaoRepositorio), typeof(AutenticacaoRepositorio));
            services.AddTransient(typeof(INotificacaoRepository), typeof(NotificacaoRepository));
            services.AddTransient(typeof(IAlunoRepositorio), typeof(AlunoRepositorio));
            services.AddTransient(typeof(IGrupoComunicadoRepository), typeof(GrupoComunicadoRepository));
            services.AddTransient(typeof(ITesteRepositorio), typeof(TesteRepositorio));

            services.AddDefaultIdentity<UsuarioAplicacao>().AddEntityFrameworkStores<AplicacaoContext>();

            services.AddIdentityServer().AddApiAuthorization<UsuarioAplicacao, AplicacaoContext>();
            services.AddTransient<IAutenticacaoService, AutenticacaoService>();

            services.AddAuthentication().AddIdentityServerJwt();
            return services;
        }
    }
}
