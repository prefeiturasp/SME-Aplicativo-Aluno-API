using Dommel;
using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class BaseRepositorio<T> : IBaseRepositorio<T> where T : EntidadeBase
    {
       
        private readonly string _connectionString;
        protected BaseRepositorio(string connectionString)
        {
            this._connectionString = connectionString;
        }

        protected virtual NpgsqlConnection InstanciarConexao()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public virtual async Task<IEnumerable<T>> ListarAsync()
        {
            using (var conexao = InstanciarConexao())
            {
                await conexao.OpenAsync();

                var retorno = await conexao.GetAllAsync<T>();

                await conexao.CloseAsync();

                return retorno;
            }
        }
        
        public virtual async Task<T> ObterPorIdAsync(long id)
        {
            using (var conexao = InstanciarConexao())
            {
                await conexao.OpenAsync();

                var retorno = await conexao.GetAsync<T>(id);

                await conexao.CloseAsync();

                return retorno;
            }
        }

        public virtual async Task RemoverAsync(long id)
        {
            using (var conexao = InstanciarConexao())
            {
                await conexao.OpenAsync();

                var entidade = await conexao.GetAsync<T>(id);

                await conexao.DeleteAsync<T>(entidade);

                await conexao.CloseAsync();
            }
        }

        public virtual async Task RemoverAsync(T entidade)
        {
            using (var conexao = InstanciarConexao())
            {
                await conexao.OpenAsync();

                await conexao.DeleteAsync<T>(entidade);

                await conexao.CloseAsync();
            }
        }
        
        public virtual async Task<long> SalvarAsync(T entidade)
        {
            if (entidade.Id == 0)
                return await InserirAsync(entidade);
           
            return await AtualizarAsync(entidade);
        }
        
        private async Task<long> InserirAsync(T entidade)
        {
            using (var conexao = InstanciarConexao())
            {
                await conexao.OpenAsync();

                entidade.InserirAuditoria();

                var id = await conexao.InsertAsync<T>(entidade);

                await conexao.CloseAsync();

                return (long)id;
            }
        }


        private async Task<long> AtualizarAsync(T entidade)
        {
            using (var conexao = InstanciarConexao())
            {
                await conexao.OpenAsync();

                entidade.AtualizarAuditoria();

                await conexao.UpdateAsync<T>(entidade);

                await conexao.CloseAsync();

                return entidade.Id;
            }
        }

    }
}
