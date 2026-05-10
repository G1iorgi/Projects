using System.Data;

namespace SharedKernel.Contracts.Abstractions.Data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
