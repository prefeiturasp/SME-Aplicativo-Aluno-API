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
                var connectionString = Environment.GetEnvironmentVariable("SGP-EA-REDIS");
                if (string.IsNullOrWhiteSpace(connectionString))
                    connectionString = configuration.GetConnectionString("SGP-EA-REDIS");

                var connectionStringCompleta = $"{connectionString},ConnectTimeout=5000";

                this.connectionMultiplexer = ConnectionMultiplexer
                    .Connect(connectionStringCompleta);
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
