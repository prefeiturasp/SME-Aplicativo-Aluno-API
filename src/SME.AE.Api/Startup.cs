using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SME.AE.Api.Configuracoes;
using SME.AE.Api.Filtros;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Infra;
using System.Linq;
using System.Text;
using SME.AE.Infra.Persistencia.Mapeamentos;
using System;
using System.Text.Unicode;

namespace SME.AE.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AddAuthentication(services, Configuration);

#if DEBUG
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
#endif

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
            });
            RegistrarMapeamentos.Registrar();
            RegistrarMvc.Registrar(services, Configuration);
        
            services.AddInfrastructure();
            services.AddApplication();

            services.AdicionarValidadoresFluentValidation();

            services.AddCors(options => options.AddDefaultPolicy(
                builder =>
                {
                    builder.WithOrigins("*");
                })
            );
            services
                .AddControllers()
                .AddNewtonsoftJson();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SME - Acompanhemento Escolar", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Por favor, entre com a palavra 'Bearer' seguido de espaço e o token JWT.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
            });

            services.AddMemoryCache();
        }
        private void AddAuthentication(IServiceCollection services,  IConfiguration configuration)
        {
            byte[] key = Encoding.ASCII.GetBytes(VariaveisAmbiente.JwtTokenSecret);
            services
                    .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                    .AddJwtBearer(x =>
                     {
                         x.RequireHttpsMetadata = false;
                         x.SaveToken = true;
                         x.TokenValidationParameters = new TokenValidationParameters
                         {
                             ValidateIssuerSigningKey = true,
                             IssuerSigningKey = new SymmetricSecurityKey(key),
                             ValidateIssuer = false,
                             ValidateAudience = false,
                         };
                     });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SME - Acompanhemento Escolar V1");
                });

            app
                    .UseCors(x => x
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod())
                    // .UseHttpsRedirection()
                    .UseAuthentication()
                    .UseRouting()
                    .UseAuthorization()
                    .UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
        }
    }
}
