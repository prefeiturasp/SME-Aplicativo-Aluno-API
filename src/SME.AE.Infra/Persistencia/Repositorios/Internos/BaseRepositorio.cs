using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class BaseRepositorio<T> : IBaseRepositorio<T> where T : EntidadeBase
    {
        private NpgsqlConnection _conexao;
        private readonly string _connectionString;

        protected NpgsqlConnection Conexao
        {
            get
            {
                if (_conexao.State == ConnectionState.Closed)
                    _conexao.Open();

                return _conexao;
            }
        }

        protected BaseRepositorio(string connectionString)
        {
            this._conexao = new NpgsqlConnection(connectionString);
            this._connectionString = connectionString;
        }

        protected virtual NpgsqlConnection InstanciarConexao()
        {
            _conexao = new NpgsqlConnection(_connectionString);

            return Conexao;
        }

        public virtual IEnumerable<T> Listar()
        {
            using (var conexao = InstanciarConexao())
            {
                var retorno = conexao.GetAll<T>();

                conexao.Close();

                return retorno;
            }
        }

        public virtual async Task<IEnumerable<T>> ListarAsync()
        {
            using (var conexao = InstanciarConexao())
            {
                var retorno = await conexao.GetAllAsync<T>();

                conexao.Close();

                return retorno;
            }
        }

        public virtual async Task<IEnumerable<T>> QueryAsync(string query, object parametros)
        {
            using (var conexao = InstanciarConexao())
            {
                var retorno = await conexao.QueryAsync<T>(query, parametros);

                conexao.Close();

                return retorno;
            }
        }

        public virtual async Task<T> QueryFirstOrDefaultAsync(string query, object parametros)
        {
            using (var conexao = InstanciarConexao())
            {
                var retorno = await conexao.QueryFirstOrDefaultAsync<T>(query, parametros);

                conexao.Close();

                return retorno;
            }
        }

        public virtual IEnumerable<T> Query(string query, object parametros)
        {
            using (var conexao = InstanciarConexao())
            {
                var retorno = conexao.Query<T>(query, parametros);

                conexao.Close();

                return retorno;
            }
        }

        public virtual T QueryFirstOrDefault(string query, object parametros)
        {
            using (var conexao = InstanciarConexao())
            {
                var retorno = conexao.QueryFirstOrDefault<T>(query, parametros);

                conexao.Close();

                return retorno;
            }
        }

        public virtual T ObterPorId(long id)
        {
            using (var conexao = InstanciarConexao())
            {
                var retorno = conexao.Get<T>(id);

                conexao.Close();

                return retorno;
            }
        }

        public virtual async Task<T> ObterPorIdAsync(long id)
        {
            using (var conexao = InstanciarConexao())
            {
                var retorno = await conexao.GetAsync<T>(id);

                conexao.Close();

                return retorno;
            }
        }

        public virtual void Remover(long id)
        {
            using (var conexao = InstanciarConexao())
            {
                var entidade = conexao.Get<T>(id);

                conexao.Delete<T>(entidade);

                conexao.Close();
            }
        }

        public virtual void Remover(T entidade)
        {
            using (var conexao = InstanciarConexao())
            {
                conexao.Delete<T>(entidade);

                conexao.Close();
            }
        }

        public virtual async Task RemoverAsync(long id)
        {
            using (var conexao = InstanciarConexao())
            {
                var entidade = await conexao.GetAsync<T>(id);

                await conexao.DeleteAsync<T>(entidade);

                conexao.Close();
            }
        }

        public virtual async Task RemoverAsync(T entidade)
        {
            using (var conexao = InstanciarConexao())
            {
                await conexao.DeleteAsync<T>(entidade);

                conexao.Close();
            }
        }

        public virtual long Salvar(T entidade)
        {
            if (entidade.Id == 0)
                return Inserir(entidade);

            return Atualizar(entidade);
        }

        public virtual async Task<long> SalvarAsync(T entidade)
        {
            if (entidade.Id == 0)
                return await InserirAsync(entidade);

            return await AtualizarAsync(entidade);
        }
        
        protected virtual void Close()
        {
            if (Conexao != null)
                _conexao.Close();
        }

        private long Inserir(T entidade)
        {
            using (var conexao = InstanciarConexao())
            {
                entidade.InserirAuditoria();
                entidade.Id = conexao.Insert<T>(entidade);

                conexao.Close();

                return entidade.Id;
            }
        }

        private async Task<long> InserirAsync(T entidade)
        {
            using (var conexao = InstanciarConexao())
            {
                entidade.InserirAuditoria();
                entidade.Id = await conexao.InsertAsync(entidade);

                conexao.Close();

                return entidade.Id;
            }
        }

        private long Atualizar(T entidade)
        {
            using (var conexao = InstanciarConexao())
            {
                entidade.AtualizarAuditoria();
                conexao.Update<T>(entidade);

                conexao.Close();

                return entidade.Id;
            }
        }

        private async Task<long> AtualizarAsync(T entidade)
        {
            using (var conexao = InstanciarConexao())
            {
                entidade.AtualizarAuditoria();
                await conexao.UpdateAsync<T>(entidade);

                conexao.Close();

                return entidade.Id;
            }
        }
    }
}
