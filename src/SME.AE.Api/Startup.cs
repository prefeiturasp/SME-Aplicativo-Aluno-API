using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SME.AE.Api.Configuracoes;
using SME.AE.Aplicacao;
using SME.AE.Infra;
using SME.AE.Infra.Persistencia.Mapeamentos;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using SME.AE.Comum;
using RabbitMQ.Client;

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
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
            });

            var variaveisGlobais = new VariaveisGlobaisOptions();
            Configuration.GetSection(nameof(VariaveisGlobaisOptions)).Bind(variaveisGlobais, c => c.BindNonPublicProperties = true);

            var servicoProdam = new ServicoProdamOptions();
            Configuration.GetSection(nameof(ServicoProdamOptions)).Bind(servicoProdam, c => c.BindNonPublicProperties = true);

            services.AddSingleton(variaveisGlobais);
            services.AddSingleton(servicoProdam);

            AddAuthentication(services, variaveisGlobais);

            services.AddApplicationInsightsTelemetry(Configuration);

            RegistrarMapeamentos.Registrar();
            RegistrarMvc.Registrar(services, variaveisGlobais);
            RegistrarClientesHttp.Registrar(services, servicoProdam, variaveisGlobais);
            ConfiguraVariaveisAmbiente(services);

            services.AddInfrastructure(variaveisGlobais);
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
            RegistrarSwagger(services);
            services.AddApplicationInsightsTelemetry();
        }

        private void ConfiguraVariaveisAmbiente(IServiceCollection services)
        {
            var configuracaoRabbitOptions = new ConfiguracaoRabbitOptions();
            Configuration.GetSection(nameof(ConfiguracaoRabbitOptions)).Bind(configuracaoRabbitOptions, c => c.BindNonPublicProperties = true);

            var rabbitConn = new ConnectionFactory
            {
                HostName = configuracaoRabbitOptions.HostName,
                UserName = configuracaoRabbitOptions.UserName,
                Password = configuracaoRabbitOptions.Password,
                VirtualHost = configuracaoRabbitOptions.VirtualHost
            };

            services.AddSingleton(rabbitConn);
        }

        private static void RegistrarSwagger(IServiceCollection services)
        {
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
        }

        private void AddAuthentication(IServiceCollection services, VariaveisGlobaisOptions variaveisGlobais)
        {
            byte[] key = Encoding.ASCII.GetBytes(variaveisGlobais.SME_AE_JWT_TOKEN_SECRET);
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
