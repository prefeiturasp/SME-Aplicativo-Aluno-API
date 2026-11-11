using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using SME.AE.Aplicacao.Servicos;
using SME.AE.Comum;
using SME.AE.Infra.Persistencia;
using SME.AE.Infra.Persistencia.Comandos;
using SME.AE.Infra.Persistencia.Repositorios;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SME.AE.Infra.Teste.Persistencia
{
    [TestFixture]
    public class InfraestruturaComportamentoTests
    {
        [Test]
        public void ComandosCoreSsoPublicosDevemConterOperacoesEParametrosEsperados()
        {
            Assert.Multiple(() =>
            {
                Assert.That(CoreSSOComandos.InserirUsuarioGrupo, Does.Contain("@usuId"));
                Assert.That(CoreSSOComandos.AtualizarStatusUsuario, Does.Contain("@status"));
                Assert.That(CoreSSOComandos.AtualizarStatusUsuarioGrupo, Does.Contain("SYS_Grupo"));
                Assert.That(CoreSSOComandos.AtualizarCriptografia, Does.Contain("@senha"));
            });
        }

        [Test]
        public void ContextoDapperDeveAbrirExporEFecharConexao()
        {
            var conexao = new FakeConnection();
            var contexto = new AplicacaoDapperContext<FakeConnection>(conexao);

            var exposta = contexto.Conexao;
            contexto.Close();

            Assert.Multiple(() =>
            {
                Assert.That(exposta, Is.SameAs(conexao));
                Assert.That(conexao.OpenCount, Is.EqualTo(1));
                Assert.That(conexao.CloseCount, Is.EqualTo(1));
            });
            contexto.Dispose();
        }

        [Test]
        public void ContextoDapperNaoDeveReabrirConexaoJaAberta()
        {
            var conexao = new FakeConnection { State = ConnectionState.Open };
            var contexto = new AplicacaoDapperContext<FakeConnection>(conexao);

            contexto.Open();

            Assert.That(conexao.OpenCount, Is.Zero);
            contexto.Dispose();
        }

        [Test]
        public void ContextoDapperDeveRejeitarAlteracaoDeBancoNaoSuportada()
        {
            var contexto = new AplicacaoDapperContext<FakeConnection>(new FakeConnection());

            Assert.That(() => contexto.ChangeDatabase("outro"), Throws.TypeOf<NotImplementedException>());
            contexto.Dispose();
        }

        [Test]
        public void CacheDeveSalvarEObterValorComGzip()
        {
            using var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var repositorio = CriarCache(memoryCache);

            repositorio.Salvar("chave", "valor", utilizarGZip: true);

            Assert.That(repositorio.Obter("chave", utilizarGZip: true), Is.EqualTo("valor"));
        }

        [Test]
        public async Task CacheDeveBuscarEArmazenarQuandoNaoEncontrado()
        {
            using var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var repositorio = CriarCache(memoryCache);
            var quantidadeBuscas = 0;

            var resultado = await repositorio.Obter("chave", () =>
            {
                quantidadeBuscas++;
                return Task.FromResult(new ValorCache { Nome = "novo" });
            });

            var armazenado = await repositorio.Obter<ValorCache>("chave", () =>
                Task.FromResult(new ValorCache { Nome = "incorreto" }));

            Assert.Multiple(() =>
            {
                Assert.That(resultado.Nome, Is.EqualTo("novo"));
                Assert.That(armazenado.Nome, Is.EqualTo("novo"));
                Assert.That(quantidadeBuscas, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task CacheDeveRetornarStringVaziaQuandoNaoEncontradoERemoverValor()
        {
            using var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var repositorio = CriarCache(memoryCache);
            await repositorio.SalvarAsync("chave", "valor");
            await repositorio.RemoverAsync("chave");

            Assert.That(await repositorio.ObterAsync("chave"), Is.Empty);
        }

        [Test]
        public void ParametroDeveRetornarFalhaParaChaveVaziaSemAcessarBanco()
        {
            var repositorio = new ParametroEscolaAquiRepositorio(new VariaveisGlobaisOptions(), null);

            Assert.Multiple(() =>
            {
                Assert.That(repositorio.TentaObterString(" ", out var texto), Is.False);
                Assert.That(texto, Is.Null);
                Assert.That(repositorio.TentaObterInt(null, out var inteiro), Is.False);
                Assert.That(inteiro, Is.Zero);
                Assert.That(repositorio.TentaObterLong("", out var longo), Is.False);
                Assert.That(longo, Is.Zero);
                Assert.That(repositorio.TentaObterDateTime(" ", out var data), Is.False);
                Assert.That(data, Is.EqualTo(DateTime.MinValue));
            });
        }

        [Test]
        public async Task ParametroDeveRetornarNuloParaListaDeChavesVazia()
        {
            var repositorio = new ParametroEscolaAquiRepositorio(new VariaveisGlobaisOptions(), null);

            var resultado = await repositorio.ObterParametros(Array.Empty<string>());

            Assert.That(resultado, Is.Null);
        }

        private static CacheRepositorio CriarCache(IMemoryCache memoryCache)
        {
            return new CacheRepositorio(new FakeServicoLog(), memoryCache);
        }

        private sealed class ValorCache
        {
            public string Nome { get; set; }
        }

        private sealed class FakeServicoLog : IServicoLog
        {
            public void Registrar(Exception ex) { }
            public void Registrar(string mensagem) { }
        }

        private sealed class FakeConnection : IDbConnection
        {
            public int OpenCount { get; private set; }
            public int CloseCount { get; private set; }
            public string ConnectionString { get; set; }
            public int ConnectionTimeout => 30;
            public string Database => "teste";
            public ConnectionState State { get; set; } = ConnectionState.Closed;

            public IDbTransaction BeginTransaction() => null;
            public IDbTransaction BeginTransaction(IsolationLevel il) => null;
            public void ChangeDatabase(string databaseName) { }
            public void Close()
            {
                CloseCount++;
                State = ConnectionState.Closed;
            }
            public IDbCommand CreateCommand() => null;
            public void Open()
            {
                OpenCount++;
                State = ConnectionState.Open;
            }
            public void Dispose() { }
        }
    }
}
