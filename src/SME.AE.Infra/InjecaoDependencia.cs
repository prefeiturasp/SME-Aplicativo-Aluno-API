using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Infra.Autenticacao;
using SME.AE.Infra.Persistencia;

namespace SME.AE.Infra
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddDbContext<AplicacaoContext>(options =>
                options.UseNpgsql(
                    ConnectionStrings.Conexao,
                    b => b.MigrationsAssembly(typeof(AplicacaoContext).Assembly.FullName)));

            services.AddScoped<IAplicacaoContext>(provider => provider.GetService<AplicacaoContext>());
            services.AddScoped<IExemploRepository>(provider => provider.GetService<ExemploRepository>());
            
            services.AddDefaultIdentity<UsuarioAplicacao>().AddEntityFrameworkStores<AplicacaoContext>();

            if (environment.IsEnvironment("Test"))
            {
                configurarServicoMockJwt(services);
            }
            else
            {
                services.AddIdentityServer()
                    .AddApiAuthorization<UsuarioAplicacao, AplicacaoContext>();

                services.AddTransient<IAutenticacaoService, AutenticacaoService>();
            }

            services.AddAuthentication().AddIdentityServerJwt();
            return services;
        }

        private static void configurarServicoMockJwt(IServiceCollection services)
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
