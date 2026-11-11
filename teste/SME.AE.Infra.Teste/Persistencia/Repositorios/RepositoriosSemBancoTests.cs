using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using SME.AE.Aplicacao.Servicos;
using SME.AE.Comum;
using SME.AE.Infra.Persistencia.Repositorios;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Teste.Persistencia.Repositorios
{
    [TestFixture]
    public class RepositoriosSemBancoTests
    {
        [Test]
        public void CacheDeveRejeitarDependenciasObrigatoriasNulas()
        {
            using var cache = new MemoryCache(new MemoryCacheOptions());
            var log = new TesteServicoLog();

            Assert.Multiple(() =>
            {
                Assert.That(() => new CacheRepositorio(null, cache), Throws.ArgumentNullException);
                Assert.That(() => new CacheRepositorio(log, null), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void ParametroDeveRejeitarOpcoesNulas()
        {
            Assert.That(() => new ParametroEscolaAquiRepositorio(null, null), Throws.ArgumentNullException);
        }

        [Test]
        public async Task CacheDeveIgnorarListaSerializadaVazia()
        {
            using var cache = new MemoryCache(new MemoryCacheOptions());
            var repositorio = new CacheRepositorio(new TesteServicoLog(), cache);

            await repositorio.SalvarAsync("lista", "[]");

            Assert.That(await repositorio.ObterAsync("lista"), Is.Empty);
        }

        [Test]
        public async Task CacheDeveRetornarPadraoQuandoFonteFalhar()
        {
            using var cache = new MemoryCache(new MemoryCacheOptions());
            var repositorio = new CacheRepositorio(new TesteServicoLog(), cache);

            var resultado = await repositorio.Obter<int>("falha", () =>
                throw new InvalidOperationException("falha controlada"));

            Assert.That(resultado, Is.Zero);
        }

        private sealed class TesteServicoLog : IServicoLog
        {
            public void Registrar(Exception ex) { }
            public void Registrar(string mensagem) { }
        }
    }
}
