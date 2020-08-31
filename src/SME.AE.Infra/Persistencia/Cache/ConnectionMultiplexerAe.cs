using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;

namespace SME.AE.Infra.Persistencia.Cache
{
    public class ConnectionMultiplexerAe : IConnectionMultiplexerAe
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;

        public ConnectionMultiplexerAe(IConfiguration configuration)
        {
            try
            {
                var teste = string.Concat(configuration.GetConnectionString("SGP-EA-REDIS"), $",ConnectTimeout={TimeSpan.FromSeconds(1).TotalMilliseconds}");

                this.connectionMultiplexer = ConnectionMultiplexer
                    .Connect(teste);
            }
            catch (RedisConnectionException rcex)
            {
                // TODO: Tratar erro de conexão
            }
            catch (Exception ex)
            {
                // TODO: Tratar erro de conexão
            }
        }

        public IDatabase GetDatabase()
        {
            if (connectionMultiplexer == null || !connectionMultiplexer.IsConnected)
                return null;

            return connectionMultiplexer.GetDatabase();
        }
    }
}
