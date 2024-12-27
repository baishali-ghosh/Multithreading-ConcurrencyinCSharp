using System;
using System.Threading;
using System.Threading.Tasks;

namespace SynchronizationContextDemo
{
    class Program
    {
        // In a console app, the Sync context is always null
        static async Task Main(string[] args)
        {
            // Check for SynchronizationContext
            var syncContext = SynchronizationContext.Current;

            if (syncContext == null)
            {
                Console.WriteLine("No SynchronizationContext is set.");
            }
            else
            {
                Console.WriteLine("SynchronizationContext is set: " + syncContext.GetType().Name);
            }

            // Call an async method to see the context behavior
            await ExampleAsyncMethod();

            // Check again after an async call
            syncContext = SynchronizationContext.Current;

            if (syncContext == null)
            {
                Console.WriteLine("No SynchronizationContext is set after async call.");
            }
            else
            {
                Console.WriteLine("SynchronizationContext is set after async call: " + syncContext.GetType().Name);
            }
        }

        static async Task ExampleAsyncMethod()
        {
            await Task.Delay(1000); // Simulate some asynchronous work
            Console.WriteLine("Inside ExampleAsyncMethod on thread: " + Environment.CurrentManagedThreadId);
        }
    }
}
