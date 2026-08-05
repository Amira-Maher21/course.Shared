

using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Shared.Kernel.BaseReturnTypes;
using Shared.Kernel.Exceptions;

namespace Shared.Infrastructure.DataContext
{
    public class DapperDbContext
    {
        private readonly IConfiguration _configuration;
        private readonly IExceptionManager _exceptionManager;
        private readonly string _connectionString;

        public DapperDbContext(IConfiguration configuration, IExceptionManager exceptionManager)
        {
            _configuration = configuration;
            _exceptionManager = exceptionManager;
            _connectionString = _configuration.GetConnectionString("Inspection");
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("No Connection string.");
            }
        }

        public async Task<ReturnBase<IEnumerable<TResult>>> QueryList<TResult>(string query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var result = await connection.QueryAsync<TResult>(query);
                    return ReturnBase<IEnumerable<TResult>>.Success(result);
                }
                catch (Exception ex)
                {
                    return ReturnBase<IEnumerable<TResult>>.Fail(ex, _exceptionManager);
                }

            }

        }

        public async Task<ReturnBase<IEnumerable<TResult>>> QueryList<TResult>(string query, Dictionary<string, object> parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var result = await connection.QueryAsync<TResult>(query, parameters);
                    //var result = await connection.QueryAsync(query, parameters);
                    return ReturnBase<IEnumerable<TResult>>.Success(result);
                    //return ReturnBase<IEnumerable<TResult>>.Success(result);
                }
                catch (Exception ex)
                {
                    return ReturnBase<IEnumerable<TResult>>.Fail(ex, _exceptionManager);
                }

            }

        }


        public async Task<ReturnBase<IEnumerable<dynamic>>> Query(string query, Dictionary<string, object> parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var result = await connection.QueryAsync(query, parameters);
                    return ReturnBase<IEnumerable<dynamic>>.Success(result);
                }
                catch (Exception ex)
                {
                    return ReturnBase<IEnumerable<dynamic>>.Fail(ex, _exceptionManager);
                }

            }

        }

        public async Task<ReturnBase<TResult>> QueryScalar<TResult>(string query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var result = await connection.QuerySingleOrDefaultAsync<TResult>(query);
                    return ReturnBase<TResult>.Success(result);
                }
                catch (Exception ex)
                {
                    return ReturnBase<TResult>.Fail(ex, _exceptionManager);
                }

            }

        }

    }
}
