using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace GestionPublica.DALC;

public class Connection
{
    private static string _connectionString;

    static Connection()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Connection>()
            .Build();

        var builder = new SqlConnectionStringBuilder()
        {
            DataSource = config["DbSettings:Server"],
            InitialCatalog = config["DbSettings:Db"],
            UserID = config["DbSettings:UserId"],
            Password = config["DbSettings:Password"],
        };

        _connectionString = builder.ConnectionString;
    }

    public static SqlConnection GetConnection()
    {
        SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }  
}