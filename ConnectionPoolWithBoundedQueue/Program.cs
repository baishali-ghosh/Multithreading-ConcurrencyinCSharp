using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var connectionPool = new ConnectionPool(5); // Create a pool with 5 connections

        // Simulate multiple clients trying to get connections
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            int taskId = i; // Capture the task ID
            tasks.Add(Task.Run(async () =>
            {
                var connection = await connectionPool.GetConnectionAsync(taskId); // Pass task ID
                Console.WriteLine($"Task {taskId}: Got connection: {connection.Id}");
                await Task.Delay(1000); // Simulate some work
                await connectionPool.ReleaseConnectionAsync(connection);
                Console.WriteLine($"Task {taskId}: Released connection: {connection.Id}");
            }));
        }

        await Task.WhenAll(tasks);
    }
}

public class DbConnection
{
    public Guid Id { get; } = Guid.NewGuid();
}

public class ConnectionPool
{
    private readonly BlockingCollection<DbConnection> _pool;
    private readonly int _maxSize;

    public ConnectionPool(int maxSize)
    {
        _maxSize = maxSize;
        _pool = new BlockingCollection<DbConnection>(new ConcurrentQueue<DbConnection>(), _maxSize);

        // Initialize the pool with connections
        for (int i = 0; i < _maxSize; i++)
        {
            _pool.Add(new DbConnection());
        }
    }

    public async Task<DbConnection> GetConnectionAsync(int taskId)
    {
        var startTime = DateTime.UtcNow; // Record the start time
        var connection = await Task.Run(() => _pool.Take());
        var waitTime = DateTime.UtcNow - startTime; // Calculate wait time
        Console.WriteLine($"Task {taskId}: Waited for {waitTime.TotalMilliseconds} ms to get connection."); // Log wait time with task ID
        return connection;
    }

    public async Task ReleaseConnectionAsync(DbConnection connection)
    {
        await Task.Run(() => _pool.Add(connection));
    }
}
