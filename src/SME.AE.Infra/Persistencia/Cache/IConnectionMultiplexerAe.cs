using StackExchange.Redis;

namespace SME.AE.Infra.Persistencia.Cache
{
    public interface IConnectionMultiplexerAe
    {
        IDatabase GetDatabase();
    }
}
