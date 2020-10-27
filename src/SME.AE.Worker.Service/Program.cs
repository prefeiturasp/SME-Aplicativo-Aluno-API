using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Infra.Persistencia.Mapeamentos;

namespace SME.AE.Worker.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RegistrarMapeamentos.Registrar();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AdicionarRepositorios()
                        .AdicionarCasosDeUso()
                        .AdicionarWorkerCasosDeUso()
                        ;
                }).ConfigureLogging((context, logging) =>
                {
                    logging.AddConfiguration(context.Configuration);
                    logging.AddSentry(option => { option.Dsn = VariaveisAmbiente.WorkerSentryDsn; });
                });
    }
}
