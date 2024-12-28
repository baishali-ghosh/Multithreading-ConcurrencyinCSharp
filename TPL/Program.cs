// See https://aka.ms/new-console-template for more information
using System;
using System.Threading.Tasks;
using System.Threading;

class Program
{
    static Action x = () => { Console.WriteLine("Sdd"); };
    static void Main(string[] args)
    {
        // Example of using TPL to run tasks in parallel
        Task task1 = Task.Run(Program.x);
        Task task2 = Task.Run(() => DoWork(2));

        Task.WaitAll(task1, task2); // Wait for both tasks to complete

        // Get the number of available threads in the thread pool
        int workerThreads, completionPortThreads;
        ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
        Console.WriteLine($"Available worker threads: {workerThreads}, Available completion port threads: {completionPortThreads}");
    }

    static void DoWork(int taskId)
    {
        Console.WriteLine($"Task {taskId} is starting on thread {Environment.CurrentManagedThreadId}.");
        // Simulate some work
        Task.Delay(1000).Wait();
        Console.WriteLine($"Task {taskId} is completed on thread {Environment.CurrentManagedThreadId}.");
    }
}
