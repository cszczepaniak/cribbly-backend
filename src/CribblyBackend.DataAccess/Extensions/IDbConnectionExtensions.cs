using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace CribblyBackend.DataAccess.Extensions
{
    public static class IDbConnectionExtensions
    {
        public static async Task<int> QueryLastInsertedId(this IDbConnection connection)
        {
            return (await connection.QueryAsync<int>("SELECT LAST_INSERT_ID()")).Single();
        }
    }
}
