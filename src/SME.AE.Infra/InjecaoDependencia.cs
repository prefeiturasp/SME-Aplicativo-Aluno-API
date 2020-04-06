using Microsoft.Extensions.DependencyInjection;

using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SME.AE.Aplicacao.Comum.Config;
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
            services.AddTransient(typeof(IExemploRepository), typeof(ExemploRepository));
            services.AddTransient(typeof(IUsuarioRepository), typeof(UsuarioRepository));
            
            services.AddDefaultIdentity<UsuarioAplicacao>().AddEntityFrameworkStores<AplicacaoContext>();

            services.AddIdentityServer().AddApiAuthorization<UsuarioAplicacao, AplicacaoContext>();
            services.AddTransient<IAutenticacaoService, AutenticacaoService>();

            services.AddAuthentication().AddIdentityServerJwt();
            return services;
        }

        private static void ConfigurarServicoMockJwt(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddApiAuthorization<UsuarioAplicacao, AplicacaoContext>(options =>
                {
                    options.Clients.Add(new Client
                    {
                        ClientId = "SME.Tests",
                        AllowedGrantTypes = { GrantType.ResourceOwnerPassword },
                        ClientSecrets = { new Secret("secret".Sha256()) },
                        AllowedScopes = { "SME.Api", "openid", "profile" }
                    });
                }).AddTestUsers(new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "f26da293-02fb-4c90-be75-e4aa51e0bb17",
                        Username = "sme@dominio.com",
                        Password = "senha",
                        Claims = new List<Claim>
                        {
                            new Claim(JwtClaimTypes.Email, "sme@dominio.com")
                        }
                    }
                });
        }
    }
}
