using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Contextos;
using SME.AE.Aplicacao.Comum.Interfaces.Geral;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class BaseRepositorio<T> : IBaseRepositorio<T> where T : EntidadeBase
    {
        protected readonly IAplicacaoDapperContext<NpgsqlConnection> database;

        protected BaseRepositorio(string connectionString)
        {
            var connection = new NpgsqlConnection(connectionString);

            this.database = new AplicacaoDapperContext<NpgsqlConnection>(connection);
        }

        public virtual IEnumerable<T> Listar()
        {
            return database.Conexao.GetAll<T>();
        }

        public virtual async Task<IEnumerable<T>> ListarAsync()
        {
            return await database.Conexao.GetAllAsync<T>();
        }

        public virtual async Task<IEnumerable<T>> QueryAsync(string query, object parametros)
        {
            return await database.Conexao.QueryAsync<T>(query, parametros);
        }

        public virtual async Task<T> QueryFirstOrDefaultAsync(string query, object parametros)
        {
            return await database.Conexao.QueryFirstOrDefaultAsync<T>(query, parametros);
        }

        public virtual IEnumerable<T> Query(string query, object parametros)
        {
            return database.Conexao.Query<T>(query, parametros);
        }

        public virtual T QueryFirstOrDefault(string query, object parametros)
        {
            return database.Conexao.QueryFirstOrDefault<T>(query, parametros);
        }

        public virtual T ObterPorId(long id)
        {
            return database.Conexao.Get<T>(id);
        }

        public virtual async Task<T> ObterPorIdAsync(long id)
        {
            return await database.Conexao.GetAsync<T>(id);
        }

        public virtual void Remover(long id)
        {
            var entidade = database.Conexao.Get<T>(id);
            database.Conexao.Delete<T>(entidade);
        }

        public virtual void Remover(T entidade)
        {
            database.Conexao.Delete<T>(entidade);
        }

        public virtual async Task RemoverAsync(long id)
        {
            var entidade = await database.Conexao.GetAsync<T>(id);
            await database.Conexao.DeleteAsync<T>(entidade);
        }

        public virtual async Task RemoverAsync(T entidade)
        {
            await database.Conexao.DeleteAsync<T>(entidade);
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

        private long Inserir(T entidade)
        {
            entidade.InserirAuditoria();
            entidade.Id = database.Conexao.Insert<T>(entidade);

            return entidade.Id;
        }

        private async Task<long> InserirAsync(T entidade)
        {
            entidade.InserirAuditoria();
            entidade.Id = await database.Conexao.InsertAsync<T>(entidade);

            return entidade.Id;
        }

        private long Atualizar(T entidade)
        {
            entidade.AtualizarAuditoria();
            database.Conexao.Update<T>(entidade);

            return entidade.Id;
        }

        private async Task<long> AtualizarAsync(T entidade)
        {
            entidade.AtualizarAuditoria();
            await database.Conexao.UpdateAsync<T>(entidade);

            return entidade.Id;
        }
    }
}
