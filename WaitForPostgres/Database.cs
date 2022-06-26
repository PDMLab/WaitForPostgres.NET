using Npgsql;

namespace WaitForPostgres;

public static class Database
{
  public static async Task WaitForConnection(
    string connectionString,
    int timeout = int.MaxValue
  )
  {
    if (timeout < 0)
    {
      throw new ArgumentException($"{nameof(timeout)} must be greater or equal to zero.");
    }

    var connection = new NpgsqlConnection(connectionString);

    var millisecondsToWait = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) + timeout;
    while (true)
    {
      try
      {
        await connection.OpenAsync();
        var command = new NpgsqlCommand(
          "SELECT 1",
          connection
        );
        await command.ExecuteNonQueryAsync();
        break;
      }
      catch (NpgsqlException e)
      {
        await connection.CloseAsync();
        await Task.Delay(500);
        var now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        if (now >= millisecondsToWait)
        {
          await connection.CloseAsync();
          NpgsqlConnection.ClearAllPools();
          throw new ApplicationException(
            $"Timeout waiting for Postgres connection {connection.Host}/{connection.Database}",
            e
          );
        }

        await connection.CloseAsync();
        NpgsqlConnection.ClearAllPools();
      }
    }
  }
}
