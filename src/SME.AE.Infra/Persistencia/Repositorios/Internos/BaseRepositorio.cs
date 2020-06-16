using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Contextos;
using SME.AE.Aplicacao.Comum.Interfaces.Geral;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class BaseRepositorio<T> : IBaseRepositorio<T> where T : EntidadeBase
    {
        private readonly NpgsqlConnection _conexao;

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
        }

        public virtual IEnumerable<T> Listar()
        {
            using (var conexao = Conexao)
            {
                var retorno = conexao.GetAll<T>();

                conexao.Close();

                return retorno;
            }
        }

        public virtual async Task<IEnumerable<T>> ListarAsync()
        {
            using (var conexao = Conexao)
            {
                var retorno = await conexao.GetAllAsync<T>();

                conexao.Close();

                return retorno;
            }
        }

        public virtual async Task<IEnumerable<T>> QueryAsync(string query, object parametros)
        {
            using (var conexao = Conexao)
            {
                var retorno = await conexao.QueryAsync<T>(query, parametros);

                conexao.Close();

                return retorno;
            }
        }

        public virtual async Task<T> QueryFirstOrDefaultAsync(string query, object parametros)
        {
            using (var conexao = Conexao)
            {
                var retorno = await conexao.QueryFirstOrDefaultAsync<T>(query, parametros);

                conexao.Close();

                return retorno;
            }
        }

        public virtual IEnumerable<T> Query(string query, object parametros)
        {
            using (var conexao = Conexao)
            {
                var retorno = conexao.Query<T>(query, parametros);

                conexao.Close();

                return retorno;
            }
        }

        public virtual T QueryFirstOrDefault(string query, object parametros)
        {
            using (var conexao = Conexao)
            {
                var retorno = conexao.QueryFirstOrDefault<T>(query, parametros);

                conexao.Close();

                return retorno;
            }
        }

        public virtual T ObterPorId(long id)
        {
            using (var conexao = Conexao)
            {
                var retorno = conexao.Get<T>(id);

                conexao.Close();

                return retorno;
            }
        }

        public virtual async Task<T> ObterPorIdAsync(long id)
        {
            using (var conexao = Conexao)
            {
                var retorno = await conexao.GetAsync<T>(id);

                conexao.Close();

                return retorno;
            }
        }

        public virtual void Remover(long id)
        {
            using (var conexao = Conexao)
            {
                var entidade = conexao.Get<T>(id);

                conexao.Delete<T>(entidade);

                conexao.Close();
            }
        }

        public virtual void Remover(T entidade)
        {
            using (var conexao = Conexao)
            {
                conexao.Delete<T>(entidade);

                conexao.Close();
            }
        }

        public virtual async Task RemoverAsync(long id)
        {
            using (var conexao = Conexao)
            {
                var entidade = await conexao.GetAsync<T>(id);

                await conexao.DeleteAsync<T>(entidade);

                conexao.Close();
            }
        }

        public virtual async Task RemoverAsync(T entidade)
        {
            using (var conexao = Conexao)
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

        public virtual void Dispose()
        {
            _conexao.Close();
        }

        protected virtual void Close()
        {
            _conexao.Close();
        }

        private long Inserir(T entidade)
        {
            using (var conexao = Conexao)
            {
                entidade.InserirAuditoria();
                entidade.Id = conexao.Insert<T>(entidade);

                conexao.Close();

                return entidade.Id;
            }
        }

        private async Task<long> InserirAsync(T entidade)
        {
            using(var conexao = Conexao)
            {
                entidade.InserirAuditoria();
                entidade.Id = await conexao.InsertAsync<T>(entidade);

                conexao.Close();

                return entidade.Id;
            }
        }

        private long Atualizar(T entidade)
        {
            using(var conexao = Conexao)
            {
                entidade.AtualizarAuditoria();
                conexao.Update<T>(entidade);

                conexao.Close();

                return entidade.Id;
            }
        }

        private async Task<long> AtualizarAsync(T entidade)
        {
            using(var conexao = Conexao)
            {
                entidade.AtualizarAuditoria();
                await conexao.UpdateAsync(entidade);

                conexao.Close();

                return entidade.Id;
            }
        }
    }
}
