using Microsoft.Extensions.DependencyInjection;

namespace SME.AE.Infra.Persistencia.Cache
{
    public static class RegistrarCache
    {
        public static void AdicionarRedis(this IServiceCollection services)
        {
            services.AddSingleton<ConnectionMultiplexerAe>();
        }
    }
}
