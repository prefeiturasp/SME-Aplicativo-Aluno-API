using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Extensions
{
    public static class DapperExtensions
    {
        public static async Task<TParent> QueryParentChildSingleAsync<TParent, TChild, TParentKey>(this IDbConnection connection, string query,
            Func<TParent, TParentKey> parentKeySelector, Func<TParent, ICollection<TChild>> childSelector, object parametros = null, IDbTransaction transaction = null,
            bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
            where TParent : class
        {
            var cache = new Dictionary<TParentKey, TParent>();

            await connection.QueryAsync<TParent, TChild, TParent>(
                query,
                (parent, child) =>
                {
                    if (!cache.ContainsKey(parentKeySelector(parent)))
                    {
                        cache.Add(parentKeySelector(parent), parent);
                    }

                    TParent cachedParent = cache[parentKeySelector(parent)];
                    ICollection<TChild> children = childSelector(cachedParent);
                    children ??= new List<TChild>();
                    children.Add(child);
                    return cachedParent;
                },
                parametros, transaction, buffered, splitOn, commandTimeout, commandType);

            return cache.Any() ? cache.First().Value : null;
        }
    }
}