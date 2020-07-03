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

        public virtual object Salvar(T entidade)
        {
            if (entidade.Id == 0)
                return Inserir(entidade);

            return Atualizar(entidade);
        }

        public virtual async Task<object> SalvarAsync(T entidade)
        {
            if (entidade.Id == 0)
                return await InserirAsync(entidade);
           
            return await AtualizarAsync(entidade);
        }



        private object Inserir(T entidade)
        {
            using (var conexao = InstanciarConexao())
            {
                entidade.InserirAuditoria();
                var entidadeRetorno = conexao.Insert(entidade);

                conexao.Close();

                return entidadeRetorno;
            }
        }

        private async Task<object> InserirAsync(T entidade)
        {
            using (var conexao = InstanciarConexao())
            {
                entidade.InserirAuditoria();
                var retornoEntidade = await conexao.InsertAsync(entidade);

                conexao.Close();

                return retornoEntidade;
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
