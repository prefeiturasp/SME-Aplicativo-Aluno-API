using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
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
                });
    }
}
