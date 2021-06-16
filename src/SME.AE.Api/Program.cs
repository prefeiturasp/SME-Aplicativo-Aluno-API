using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SME.AE.Comum;

namespace SME.AE.Api
{
    public class Program
    {
        private static string SentryDsn;
        private readonly VariaveisGlobaisOptions variaveisGlobais;

        public Program(VariaveisGlobaisOptions variaveisGlobais) 
        {
            this.variaveisGlobais = variaveisGlobais ?? throw new System.ArgumentNullException(nameof(variaveisGlobais));
        }

        public static void Main(string[] args)
        {
            SentryDsn = variaveisGlobais.SentryDsn;
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddUserSecrets<Program>();
            })
            .UseStartup<Startup>()
            .UseSentry(option => { option.Dsn = SentryDsn; })
            .UseUrls("http://0.0.0.0:5000;");
    }
}