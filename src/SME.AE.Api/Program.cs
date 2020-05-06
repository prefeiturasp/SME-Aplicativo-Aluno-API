using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces;

namespace SME.AE.Api
{
    public class Program
    {

        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseKestrel(c =>
                    {
                        c.ConfigureHttpsDefaults(opt =>
                        {
                            opt.SslProtocols = SslProtocols.Tls;
                        });
                    })
                    .UseUrls("http://0.0.0.0:5000;https://0.0.0.0:5001;")
                    .UseStartup<Startup>()
                    .UseSentry(option => { option.Dsn = VariaveisAmbiente.SentryDsn; });
            });
    }
}
