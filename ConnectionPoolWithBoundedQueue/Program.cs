using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using System.Diagnostics;

class Program
{
    // Create a connection pool with a bounded queue
    // Simulate a scenario where we have a large number of concurrent connections
    // and we want to limit the number of concurrent connections to the database
    // to avoid overwhelming the database with too many concurrent connections
    // Use 3 to benchmark the number of connections that can be opened without timing out. If you want to see the breakage, set it to 200
    // Use 4 to benchmark the number of operations that can be performed with a pool of 10 connections
    static async Task Main(string[] args)
    {
        Console.WriteLine("Choose mode:");
        Console.WriteLine("1. Simulated Connection Pool");
        Console.WriteLine("2. Real SQL Connection Pool");
        Console.WriteLine("3. Benchmark 1000 connections without pool");
        Console.WriteLine("4. Benchmark 1000 connections with pool");
        Console.Write("Enter your choice (1, 2, 3 or 4): ");
        int mode = int.Parse(Console.ReadLine());
        int maxConnections = 1;
        if (mode == 1 || mode == 2) {
            Console.WriteLine("Enter the maximum number of connections:");
             maxConnections = int.Parse(Console.ReadLine());
        }
        ConnectionPool<SimulatedConnection> simulationPool = null;
        ConnectionPool<DbConnection> dbPool = null;

        string connectionString = "Server=localhost;Database=baishali_test;User ID=root;Port=3306;";
        switch (mode)
        {
            case 1:
                simulationPool = new ConnectionPool<SimulatedConnection>(maxConnections, connectionString);
                break;
            case 2:
                dbPool = new ConnectionPool<DbConnection>(maxConnections, connectionString);
                break;
            case 3:
                await BenchmarkWithoutPool(connectionString);
                return;
            case 4:
                await BenchmarkWithPool(connectionString);
                return;
            default:
                Console.WriteLine("Invalid mode selected.");
                break;
        }

        // Simulate multiple clients trying to get connections
        var tasks = new List<Task>();
        for (int i = 0; i < maxConnections; i++)
        {
            int taskId = i;
            tasks.Add(Task.Run(async () =>
            {
                IConnection connection;
                if (simulationPool != null)
                {
                    connection = await simulationPool.GetConnectionAsync(taskId);
                }
                else
                {
                    connection = await dbPool.GetConnectionAsync(taskId);
                }
                Console.WriteLine($"Task {taskId}: Got connection: {connection.Id}");
                await connection.DoWork();
                if (simulationPool != null)
                {
                    await simulationPool.ReleaseConnectionAsync((SimulatedConnection)connection);
                }
                else
                {
                    await dbPool.ReleaseConnectionAsync((DbConnection)connection);
                }
                Console.WriteLine($"Task {taskId}: Released connection: {connection.Id}");
            }));
        }

        await Task.WhenAll(tasks);
    }

    static async Task BenchmarkWithoutPool(string connectionString)
    {
        int concurrentConnections = 1000;
        int numberOfConnectionsToTimeoutSql = 200;
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var tasks = new List<Task>();
        for (int i = 0; i < concurrentConnections; i++)
        {
            int taskId = i;
            tasks.Add(Task.Run(async () => 
            {
                try{
                    Console.WriteLine($"Task {taskId +1}: Executing");
                    var connection = new MySqlConnection(connectionString);
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand("SELECT SLEEP(0.10)", connection))
                    {
                        await command.ExecuteNonQueryAsync();
                        }
                    } 
                catch (MySqlException ex)
                {
                    Console.WriteLine($"Task {taskId +1}: Error: {ex.Message}");
                }
                finally
                {
                    // To simulate the breakage, we need to keep the connection open
                    // 151 is the mysql limit, so we need to have > 151 open concurrent 
                    // connections to see the breakage
                    //await connection.CloseAsync();
                }
            }));
        }

        await Task.WhenAll(tasks);

        stopwatch.Stop();
        Console.WriteLine($"Benchmark completed. Total time: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Average time per connection: {stopwatch.ElapsedMilliseconds / (double)concurrentConnections} s");
    }

    static async Task BenchmarkWithPool(string connectionString)
    {
        int numberOfOperations = 50;
        int poolSize = 4;
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var tasks = new List<Task>();
        var connectionPool = new ConnectionPool<DbConnection>(poolSize, connectionString);

        for (int i = 0; i < numberOfOperations; i++)
        {
            int taskId = i;
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var connection = await connectionPool.GetConnectionAsync(taskId);
                    Console.WriteLine($"Task {taskId}: Got connection: {connection.Id}");
                    try
                    {
                        await connection.DoWork();
                        Console.WriteLine($"Task {taskId}: Operation completed");
                    }
                    finally
                    {
                        await connectionPool.ReleaseConnectionAsync(connection);
                        Console.WriteLine($"Task {taskId}: Connection released");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Task {taskId}: Error: {ex.Message}");
                }
            }));
        }

        await Task.WhenAll(tasks);

        stopwatch.Stop();
        Console.WriteLine($"Benchmark completed. Total time: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Average time per operation: {stopwatch.ElapsedMilliseconds / (double)numberOfOperations} ms");
    }

}

public interface IConnection
{
    Guid Id { get; }

    Task DoWork();
}

public class SimulatedConnection : IConnection
{
    public Guid Id { get; } = Guid.NewGuid();

    public SimulatedConnection()
    {
        Console.WriteLine($"SimulatedConnection {Id} created");
    }

    public SimulatedConnection(string connectionString)
    {
        Console.WriteLine($"SimulatedConnection {connectionString} created");
    }

    public async Task DoWork()
    {
        //Console.WriteLine($"SimulatedConnection {Id} is doing work");
        await Task.Delay(1000);
    }
}

public class DbConnection : IConnection
{
    public Guid Id { get; } = Guid.NewGuid();
    public MySqlConnection Connection { get; private set; }

    public DbConnection()
    {
        Console.WriteLine($"DbConnection {Id} created");
    }

    public DbConnection(string connectionString)
    {
        Connection = new MySqlConnection(connectionString);
        Console.WriteLine($"DbConnection is initialized");
    }
    public async Task DoWork()
    {
        try
        {
            Console.WriteLine($"DbConnection {Id}: Attempting to open connection");
            await Connection.OpenAsync();
            //Console.WriteLine($"DbConnection {Id}: Connection opened successfully");
            var command = new MySqlCommand("SELECT SLEEP(0.01)", Connection);
            await command.ExecuteNonQueryAsync();
            Console.WriteLine($"DbConnection {Id}: Query completed");
            await Connection.CloseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DbConnection {Id}: Error in DoWork: {ex.Message}");
            Console.WriteLine($"DbConnection {Id}: Stack trace: {ex.StackTrace}");
        }
        finally
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine($"DbConnection {Id}: Closing connection");
                await Connection.CloseAsync();
                Console.WriteLine($"DbConnection {Id}: Connection closed");
            }
        }
    }

    public void Initialize(string connectionString)
    {
        Console.WriteLine($"DbConnection is initializing");
        Connection = new MySqlConnection(connectionString);
        Console.WriteLine($"DbConnection is initialized");
    }
}


public class ConnectionPool<TConnection> where TConnection : class, IConnection, new()
{
    private readonly BlockingCollection<TConnection> _pool;
    private readonly int _maxSize;
    private readonly string _connectionString;

    public ConnectionPool(int maxSize, string connectionString)
    {
        _maxSize = maxSize;
        _connectionString = connectionString;
        _pool = new BlockingCollection<TConnection>(new ConcurrentQueue<TConnection>(), _maxSize);

        // Initialize the pool with connections
        for (int i = 0; i < _maxSize; i++)
        {
            var connection = new TConnection();
            if (connection is SimulatedConnection simulatedConnection)
            {
                // Do nothing, SimulatedConnection doesn't need initialization
            }
            else if (connection is DbConnection dbConnection)
            {
                dbConnection.Initialize(_connectionString);
            }
            _pool.Add(connection);
        }
    }

    public async Task<TConnection> GetConnectionAsync(int taskId)
    {
        //var startTime = DateTime.UtcNow; // Record the start time
        var connection = _pool.Take();
        //var waitTime = DateTime.UtcNow - startTime; // Calculate wait time
        //Console.WriteLine($"Task {taskId}: Waited for {waitTime.TotalMilliseconds} ms to get connection."); // Log wait time with task ID
        return connection;
    }

    public async Task ReleaseConnectionAsync(TConnection connection)
    {
        _pool.Add(connection);
    }
}



