using Dommel;
using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class BaseRepositorio<T> : IBaseRepositorio<T> where T : EntidadeBase
    {
        private readonly string connectionString;

        protected BaseRepositorio(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected virtual NpgsqlConnection InstanciarConexao()
        {
            return new NpgsqlConnection(connectionString);
        }

        public virtual async Task<IEnumerable<T>> ListarAsync()
        {
            await using var conexao = InstanciarConexao();
            await conexao.OpenAsync();

            var retorno = await conexao.GetAllAsync<T>();

            await conexao.CloseAsync();

            return retorno;
        }

        public virtual async Task<T> ObterPorIdAsync(long id)
        {
            await using var conexao = InstanciarConexao();
            await conexao.OpenAsync();

            var retorno = await conexao.GetAsync<T>(id);

            await conexao.CloseAsync();

            return retorno;
        }

        public virtual async Task RemoverAsync(long id)
        {
            await using var conexao = InstanciarConexao();
            await conexao.OpenAsync();

            var entidade = await conexao.GetAsync<T>(id);
            if (entidade != null)
            {
                await conexao.DeleteAsync(entidade);
            }

            await conexao.CloseAsync();
        }

        public virtual async Task RemoverAsync(T entidade)
        {
            await using var conexao = InstanciarConexao();
            await conexao.OpenAsync();

            await conexao.DeleteAsync(entidade);

            await conexao.CloseAsync();
        }

        public virtual async Task<long> SalvarAsync(T entidade)
        {
            if (entidade.Id == 0)
                return await InserirAsync(entidade);

            return await AtualizarAsync(entidade);
        }

        private async Task<long> InserirAsync(T entidade)
        {
            await using var conexao = InstanciarConexao();
            await conexao.OpenAsync();

            entidade.InserirAuditoria();

            var id = await conexao.InsertAsync(entidade);

            await conexao.CloseAsync();

            return Convert.ToInt64(id);
        }

        private async Task<long> AtualizarAsync(T entidade)
        {
            await using var conexao = InstanciarConexao();
            await conexao.OpenAsync();

            entidade.AtualizarAuditoria();

            await conexao.UpdateAsync(entidade);

            await conexao.CloseAsync();

            return entidade.Id;
        }
    }
}