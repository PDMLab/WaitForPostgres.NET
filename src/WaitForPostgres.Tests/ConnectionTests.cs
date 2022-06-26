using DockerCompose;
using Npgsql;
using static WaitForPostgres.Database;

namespace WaitForPostgres.Tests;

public class ConnectionTests
{
  [Fact]
  public async Task ShouldWaitForPostgresConnection()
  {
    var file = new DirectoryInfo(
      Directory.GetCurrentDirectory()
    );
    var compose = new Compose(file);

    await compose.Up();

    var connectionStringBuilder = new NpgsqlConnectionStringBuilder()
    {
      Pooling = false,
      Port = 5432,
      Host = "localhost",
      CommandTimeout = 20,
      Database = "postgres",
      Password = "123456",
      Username = "postgres"
    };
    var pgTestConnectionString = connectionStringBuilder.ToString();

    await WaitForConnection(pgTestConnectionString, 10000);
    await compose.Down();
  }

  [Fact]
  public async Task ShouldThrowOnTimeoutWaitingForPostgresConnection()
  {
    var connectionStringBuilder = new NpgsqlConnectionStringBuilder()
    {
      Pooling = false,
      Port = 5432,
      Host = "localhost",
      CommandTimeout = 20,
      Database = "postgres",
      Password = "123456",
      Username = "postgres"
    };
    var pgTestConnectionString = connectionStringBuilder.ToString();

    await Assert.ThrowsAsync<ApplicationException>(async () => await WaitForConnection(pgTestConnectionString, 10000));
  }
}
