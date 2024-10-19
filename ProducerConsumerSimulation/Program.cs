using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static BlockingCollection<int> buffer = new BlockingCollection<int>(boundedCapacity: 5);
    static Random random = new Random();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Producer-Consumer Simulation");
        Console.WriteLine("Press any key to stop the simulation.");

        Task producerTask = Task.Run(Producer);
        Task consumerTask = Task.Run(Consumer);

        await Task.WhenAny(producerTask, consumerTask, Task.Run(() => Console.ReadKey()));

        buffer.CompleteAdding();
        await Task.WhenAll(producerTask, consumerTask);

        Console.WriteLine("Simulation ended.");
    }

    static void Producer()
    {
        try
        {
            while (true)
            {
                int item = random.Next(1, 100);
                buffer.Add(item);
                Console.WriteLine($"Produced: {item}");
                Thread.Sleep(random.Next(500, 1500));
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("Producer finished.");
        }
    }

    static void Consumer()
    {
        try
        {
            foreach (var item in buffer.GetConsumingEnumerable())
            {
                Console.WriteLine($"Consumed: {item}");
                Thread.Sleep(random.Next(500, 1500));
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("Consumer finished.");
        }
    }
}
