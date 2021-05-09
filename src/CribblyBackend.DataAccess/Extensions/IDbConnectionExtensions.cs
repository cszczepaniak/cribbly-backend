using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Common;
using Dapper;

namespace CribblyBackend.DataAccess.Extensions
{
    public static class IDbConnectionExtensions
    {
        public static async Task<int> QueryLastInsertedId(this IDbConnection connection)
        {
            return (await connection.QueryAsync<int>("SELECT LAST_INSERT_ID()")).Single();
        }

        public static async Task<IEnumerable<TReturn>> QueryWithObjectAsync<T1, T2, TReturn>(
            this IDbConnection connection,
            Query query,
            Func<T1, T2, TReturn> map
        )
        {
            return await connection.QueryAsync<T1, T2, TReturn>(query.Sql, map, param: query.Params);
        }
        public static async Task<IEnumerable<TReturn>> QueryWithObjectAsync<T1, T2, T3, TReturn>(
            this IDbConnection connection,
            Query query,
            Func<T1, T2, T3, TReturn> map
        )
        {
            return await connection.QueryAsync<T1, T2, T3, TReturn>(query.Sql, map, param: query.Params);
        }
        public static async Task<IEnumerable<TReturn>> QueryWithObjectAsync<TReturn>(this IDbConnection connection, Query query)
        {
            return await connection.QueryAsync<TReturn>(query.Sql, query.Params);
        }
        public static async Task<int> ExecuteWithObjectAsync(this IDbConnection connection, Query query)
        {
            return await connection.ExecuteAsync(query.Sql, query.Params);
        }
    }
}
