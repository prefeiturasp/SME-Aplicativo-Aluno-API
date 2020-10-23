using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SME.AE.Aplicacao.Comum.Config;

namespace SME.AE.Worker.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSentry(option => { option.Dsn = VariaveisAmbiente.SentryDsn; });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AdicionarRepositorios()
                        .AdicionarCasosDeUso()
                        .AdicionarWorkerCasosDeUso()
                        ;
                });
    }
}
