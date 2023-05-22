using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PayBill.UserManagement.Repositories.Infrastructure;

namespace PayBill.UserManagement.Repositories.Persistence;

public class DataAccessHelper : IDataAccessHelper
{
    private readonly IConfiguration _config;

    public DataAccessHelper(IConfiguration config)
    {
        _config = config;
    }

    public async Task<List<T>> QueryData<T, TU>(string storedProcedure, TU parameters)
    {
        using (IDbConnection connection = new SqlConnection(_config.GetConnectionString("MSSQL")))
        {
            var rows = await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            return rows.ToList();
        }
    }

    public async Task<int> ExecuteData<T>(string storedProcedure, T parameters)
    {
        using (IDbConnection connection = new SqlConnection(_config.GetConnectionString("MSSQL")))
        {
            return await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}