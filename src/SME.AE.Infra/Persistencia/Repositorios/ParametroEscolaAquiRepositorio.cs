using Dapper;
using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class ParametroEscolaAquiRepositorio : IParametrosEscolaAquiRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public ParametroEscolaAquiRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        private NpgsqlConnection InstanciarConexao()
        {
            return new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);
        }

        private async Task SalvarAsync(string chave, string conteudo)
        {
            if (string.IsNullOrWhiteSpace(chave))
            {
                return;
            }

            using var conexao = InstanciarConexao();

            await conexao.OpenAsync();

            if (string.IsNullOrWhiteSpace(conteudo))
            {
                await conexao.ExecuteAsync("delete from parametroescolaaqui where chave = @chave", new { chave });
            }
            else
            {
                await conexao.ExecuteAsync(
                    @"
                        insert into parametroescolaaqui (chave, conteudo)
                        values (@chave, @conteudo)
                        on conflict (chave)
                        do update set conteudo = excluded.conteudo
                    ", new { chave, conteudo });
            }

            await conexao.CloseAsync();
        }

        private async Task<string> ObterPorChaveAsync(string chave)
        {
            if (string.IsNullOrWhiteSpace(chave))
            {
                return null;
            }

            using var conexao = InstanciarConexao();
            await conexao.OpenAsync();
            var conteudoString = await conexao.QueryFirstOrDefaultAsync<string>("select conteudo from parametroescolaaqui where chave = @chave", new { chave });
            await conexao.CloseAsync();
            return conteudoString;
        }

        public bool TentaObterString(string chave, out string conteudo)
        {
            string conteudoString = null;
            Task.Run(
                async () =>
                {
                    conteudoString = await ObterPorChaveAsync(chave);
                }
            ).Wait();
            conteudo = conteudoString;
            return !string.IsNullOrWhiteSpace(conteudo);
        }

        public bool TentaObterInt(string chave, out int conteudo)
        {
            conteudo = 0;
            if (TentaObterString(chave, out var conteudoString))
                return int.TryParse(conteudoString, out conteudo);
            return false;
        }

        public bool TentaObterLong(string chave, out long conteudo)
        {
            conteudo = 0;
            if (TentaObterString(chave, out var conteudoString))
                return long.TryParse(conteudoString, out conteudo);
            return false;
        }

        public bool TentaObterDateTime(string chave, out DateTime conteudo)
        {
            conteudo = DateTime.MinValue;
            if (TentaObterString(chave, out var conteudoString))
                return DateTime.TryParseExact(
                    conteudoString,
                    @"yyyy-MM-dd\THH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out conteudo);
            return false;
        }

        public void Salvar(string chave, string conteudo) => Task.Run(async () => await SalvarAsync(chave, conteudo)).Wait();

        public void Salvar(string chave, int conteudo) => Salvar(chave, conteudo.ToString());

        public void Salvar(string chave, long conteudo) => Salvar(chave, conteudo.ToString());

        public void Salvar(string chave, DateTime conteudo) => Salvar(chave, conteudo.ToString("s"));

        public DateTime? ObterDateTime(string chave)
        {
            if (TentaObterDateTime(chave, out var conteudo))
            {
                return conteudo;
            }
            return null;
        }

        public int? ObterInt(string chave)
        {
            if (TentaObterInt(chave, out var conteudo))
            {
                return conteudo;
            }
            return null;
        }

        public long? ObterLong(string chave)
        {
            if (TentaObterLong(chave, out var conteudo))
            {
                return conteudo;
            }
            return null;
        }

        public string ObterString(string chave)
        {
            if (TentaObterString(chave, out var conteudo))
            {
                return conteudo;
            }
            return null;
        }

        public async Task<IEnumerable<ParametroEscolaAqui>> ObterParametros(IEnumerable<string> chaves)
        {
            var chavesConcatenadas = string.Join("','", chaves);
            if (string.IsNullOrWhiteSpace(chavesConcatenadas)) return null;

            using var conexao = InstanciarConexao();
            await conexao.OpenAsync();
            var parametros = await conexao.QueryAsync<ParametroEscolaAqui>($"select chave, conteudo from parametroescolaaqui where chave IN ('{chavesConcatenadas}')");
            await conexao.CloseAsync();
            return parametros;
        }

        public DateTime ObterDateTime(string chave, DateTime padrao)
        {
            if (TentaObterDateTime(chave, out var conteudo))
            {
                return conteudo;
            }
            else
            {
                Salvar(chave, padrao);
                return padrao;
            }
        }

        public int ObterInt(string chave, int padrao)
        {
            if (TentaObterInt(chave, out var conteudo))
            {
                return conteudo;
            }
            else
            {
                Salvar(chave, padrao);
                return padrao;
            }
        }

        public long ObterLong(string chave, long padrao)
        {
            if (TentaObterLong(chave, out var conteudo))
            {
                return conteudo;
            }
            else
            {
                Salvar(chave, padrao);
                return padrao;
            }
        }

        public string ObterString(string chave, string padrao)
        {
            if (TentaObterString(chave, out var conteudo))
            {
                return conteudo;
            }
            else
            {
                Salvar(chave, padrao);
                return padrao;
            }
        }
    }
}