using Dapper;
using Dommel;
using SME.AE.Aplicacao.Comum.Interfaces.Contextos;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades.Externas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class ExternoRepositorio<T,Z> : IExternoRepositorio<T,Z> where T : EntidadeExterna where Z : IDbConnection , IDisposable
    {
        protected IAplicacaoDapperContext<Z> database;

        protected ExternoRepositorio(Z connection)
        {            
            database = new AplicacaoDapperContext<Z>(connection);
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

        public void Dispose() => database.Conexao.Close();
    }
}
