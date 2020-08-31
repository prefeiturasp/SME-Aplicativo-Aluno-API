using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Compressao;
using SME.AE.Infra.Persistencia.Cache;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class CacheRepositorio : ICacheRepositorio
    {
        private readonly IConnectionMultiplexerAe connectionMultiplexerAe;

        public CacheRepositorio(IConnectionMultiplexerAe connectionMultiplexerAe)
        {
            this.connectionMultiplexerAe = connectionMultiplexerAe ?? throw new ArgumentNullException(nameof(connectionMultiplexerAe));
        }

        public string Obter(string nomeChave, bool utilizarGZip = false)
        {
            var inicioOperacao = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var cacheParaRetorno = connectionMultiplexerAe.GetDatabase()?.StringGet(nomeChave);
                timer.Stop();
                if (utilizarGZip)
                {
                    cacheParaRetorno = UtilGZip.Descomprimir(Convert.FromBase64String(cacheParaRetorno));
                }

                return cacheParaRetorno;
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                timer.Stop();
                return null;
            }
        }

        public T Obter<T>(string nomeChave, bool utilizarGZip = false)
        {
            try
            {
                var stringCache = connectionMultiplexerAe.GetDatabase()?.StringGet(nomeChave).ToString();
                if (!string.IsNullOrWhiteSpace(stringCache))
                {
                    if (utilizarGZip)
                    {
                        stringCache = UtilGZip.Descomprimir(Convert.FromBase64String(stringCache));
                    }
                    return JsonConvert.DeserializeObject<T>(stringCache);
                }
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
            }
            return default(T);
        }

        public async Task<T> Obter<T>(string nomeChave, Func<Task<T>> buscarDados, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            try
            {
                var stringCache = connectionMultiplexerAe.GetDatabase()?.StringGet(nomeChave).ToString();
                if (!string.IsNullOrWhiteSpace(stringCache))
                {
                    if (utilizarGZip)
                    {
                        stringCache = UtilGZip.Descomprimir(Convert.FromBase64String(stringCache));
                    }
                    return JsonConvert.DeserializeObject<T>(stringCache);
                }

                var dados = await buscarDados();

                await SalvarAsync(nomeChave, JsonConvert.SerializeObject(dados), minutosParaExpirar, utilizarGZip);

                return dados;
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                return await buscarDados();
            }
        }

        public async Task<T> ObterAsync<T>(string nomeChave, Func<Task<T>> buscarDados, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            var inicioOperacao = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var dbCache = connectionMultiplexerAe.GetDatabase();
                var stringCache = dbCache != null ? await connectionMultiplexerAe.GetDatabase()?.StringGetAsync(nomeChave) : RedisValue.Null;

                timer.Stop();
                // servicoLog.RegistrarDependenciaAppInsights("Redis", nomeChave, "Obtendo Async", inicioOperacao, timer.Elapsed, true);

                if (!stringCache.IsNullOrEmpty)
                {
                    if (utilizarGZip)
                    {
                        stringCache = UtilGZip.Descomprimir(Convert.FromBase64String(stringCache));
                    }
                    return JsonConvert.DeserializeObject<T>(stringCache);
                }

                var dados = await buscarDados();

                await SalvarAsync(nomeChave, dados, minutosParaExpirar, utilizarGZip);

                return dados;
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                timer.Stop();
                return await buscarDados();
            }
        }

        public async Task<string> ObterAsync(string nomeChave, bool utilizarGZip = false)
        {
            var inicioOperacao = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var dbCache = connectionMultiplexerAe.GetDatabase();
                var cacheParaRetorno = dbCache != null ? await dbCache.StringGetAsync(nomeChave) : RedisValue.Null;
                timer.Stop();
                //servicoLog.RegistrarDependenciaAppInsights("Redis", nomeChave, "Obtendo async", inicioOperacao, timer.Elapsed, true);

                if (!cacheParaRetorno.IsNullOrEmpty && utilizarGZip)
                    cacheParaRetorno = UtilGZip.Descomprimir(Convert.FromBase64String(cacheParaRetorno));

                return cacheParaRetorno;
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                //servicoLog.Registrar(ex);
                timer.Stop();
                //servicoLog.RegistrarDependenciaAppInsights("Redis", nomeChave, $"Obtendo async - Erro {ex.Message}", inicioOperacao, timer.Elapsed, false);
                return null;
            }
        }

        public async Task RemoverAsync(string nomeChave)
        {
            var inicioOperacao = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var dbCache = connectionMultiplexerAe.GetDatabase();

                if (dbCache == null)
                    return;

                await dbCache.KeyDeleteAsync(nomeChave);
                timer.Stop();
                //servicoLog.RegistrarDependenciaAppInsights("Redis", nomeChave, "Remover async", inicioOperacao, timer.Elapsed, true);
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                timer.Stop();
                //servicoLog.RegistrarDependenciaAppInsights("Redis", nomeChave, "Remover async", inicioOperacao, timer.Elapsed, false);
                // servicoLog.Registrar(ex);
            }
        }

        public void Salvar(string nomeChave, string valor, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            try
            {
                var dbCache = connectionMultiplexerAe.GetDatabase();

                if (dbCache == null)
                    return;

                if (utilizarGZip)
                {
                    var valorComprimido = UtilGZip.Comprimir(valor);
                    valor = Convert.ToBase64String(valorComprimido);
                }

                var redisKey = new RedisKey(nomeChave);
                var rediValue = new RedisValue(valor);

                dbCache.StringSet(redisKey, rediValue, TimeSpan.FromMinutes(minutosParaExpirar));
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                //servicoLog.Registrar(ex);
            }
        }

        public async Task SalvarAsync(string nomeChave, string valor, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            var inicioOperacao = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var dbCache = connectionMultiplexerAe.GetDatabase();

                if (dbCache == null)
                    return;

                if (!string.IsNullOrWhiteSpace(valor) && valor != "[]")
                {
                    if (utilizarGZip)
                    {
                        var valorComprimido = UtilGZip.Comprimir(valor);
                        valor = Convert.ToBase64String(valorComprimido);
                    }

                    await dbCache.StringSetAsync(nomeChave, valor, TimeSpan.FromMinutes(minutosParaExpirar));

                    timer.Stop();
                    //servicoLog.RegistrarDependenciaAppInsights("Redis", nomeChave, "Salvar async", inicioOperacao, timer.Elapsed, true);
                }
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                timer.Stop();
                //servicoLog.RegistrarDependenciaAppInsights("Redis", nomeChave, "Salvar async", inicioOperacao, timer.Elapsed, false);
                //servicoLog.Registrar(ex);
            }
        }

        public async Task SalvarAsync(string nomeChave, object valor, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            await SalvarAsync(nomeChave, JsonConvert.SerializeObject(valor), minutosParaExpirar, utilizarGZip);
        }
    }
}
