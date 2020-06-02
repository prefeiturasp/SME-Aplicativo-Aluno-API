using Dapper;
using Dapper.Contrib.Extensions;
using SME.AE.Aplicacao.Comum.Interfaces.Contextos;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class ExternoRepositorio<T> : IExternoRepositorio<T> where T : class
    {
        protected IAplicacaoDapperContext database;

        protected ExternoRepositorio(string connectionString)
        {
            database = new AplicacaoDapperContext(connectionString);
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
    }
}
