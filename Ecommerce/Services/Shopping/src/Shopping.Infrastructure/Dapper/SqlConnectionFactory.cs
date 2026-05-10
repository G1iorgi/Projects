using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;
using SharedKernel.Configurations;
using SharedKernel.Contracts.Abstractions.Data;

namespace Shopping.Infrastructure.Dapper;

public class SqlConnectionFactory(IOptions<ConnectionStringsOptions> options) : ISqlConnectionFactory
{
    private readonly ConnectionStringsOptions _options = options.Value;

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_options.ShoppingDbContextMaster);
    }
}
