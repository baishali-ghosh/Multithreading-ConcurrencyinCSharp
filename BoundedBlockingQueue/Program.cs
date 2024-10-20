using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

// Bounded Blocking Queue without using any libraries
// Implement a thread-safe bounded blocking queue with the following methods:
//
// - BoundedBlockingQueue(int capacity): Initializes the queue with a maximum capacity.
// - void enqueue(int element): Adds an element to the front of the queue. 
//   If the queue is full, the calling thread is blocked until space is available.
// - int dequeue(): Returns and removes the element at the rear of the queue. 
//   If the queue is empty, the calling thread is blocked until an element is available.
// - int size(): Returns the current number of elements in the queue.
//
// This implementation will be tested with multiple concurrent producer and consumer threads.
// Producer threads will only call enqueue, while consumer threads will only call dequeue.
// The size method will be called after each test case.

// Example input:
// Example input:
// Enter the number of producer threads:
// 3
// Enter the number of consumer threads:
// 4
// Enter the operations (comma-separated):
// "BoundedBlockingQueue","enqueue","enqueue","enqueue","dequeue","dequeue","dequeue","enqueue"
// Enter the arguments (comma-separated):
// 3,1,0,2,,,,3 
// Output: [1,0,2]


partial class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter the number of producer threads:");
        int producerThreads = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter the number of consumer threads:");
        int consumerThreads = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter the operations (comma-separated):");
        string[] operations = Console.ReadLine().Trim('[', ']').Split(',');

        Console.WriteLine("Enter the arguments (comma-separated):");
        string[] arguments = Console.ReadLine().Trim('[', ']').Split(',');

        BoundedBlockingQueue queue = null;
        var output = new List<int>();

        for (int i = 0; i < operations.Length; i++)
        {
            string op = operations[i].Trim('"');
            switch (op)
            {
                case "BoundedBlockingQueue":
                    int capacity = int.Parse(arguments[i].Trim('[', ']'));
                    queue = new BoundedBlockingQueue(capacity);
                    break;
                case "enqueue":
                    int element = int.Parse(arguments[i].Trim('[', ']'));
                    await Task.Run(() => queue.enqueue(element));
                    break;
                case "dequeue":
                    output.Add(await Task.Run(() => queue.dequeue()));
                    break;
                case "size":
                    output.Add(queue.size());
                    break;
            }
        }

        Console.WriteLine($"Output: [{string.Join(",", output)}]");
    }
}

public class BoundedBlockingQueue
{
    private readonly int[] buffer;
    private SemaphoreSlim enqueueSemaphore;
    private SemaphoreSlim dequeueSemaphore;
    private Queue<int> queue = new Queue<int>();
    private readonly object _lock = new object();

    public BoundedBlockingQueue(int capacity)
    {
        buffer = new int[capacity];
        enqueueSemaphore = new SemaphoreSlim(capacity);
        dequeueSemaphore = new SemaphoreSlim(0);
    }

    public void enqueue(int element)
    {
        enqueueSemaphore.Wait();
        lock (_lock)
        {
            // This is critical section and thread safe
            queue.Enqueue(element);
        }
        dequeueSemaphore.Release();
    }

    public int dequeue()
    {
        dequeueSemaphore.Wait();
        int element;
        lock (_lock)
        {
            element = queue.Dequeue();
        }
        enqueueSemaphore.Release();
        return element;
    }

    public int size()
    {
        lock (_lock)
        {
            return queue.Count;
        }
    }
}
