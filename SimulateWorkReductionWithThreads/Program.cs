// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

class Program
{
    static void Main()
    {
        const long totalNumbers = 10_000_000;
        const int iterations = 100;

        // Warm-up run
        CalculateWithoutThreads(totalNumbers, iterations);
        CalculateWithThreads(totalNumbers, iterations);

        // Actual timed runs
        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();
        double resultWithoutThreads = CalculateWithoutThreads(totalNumbers, iterations);
        stopwatch.Stop();
        TimeSpan timeWithoutThreads = stopwatch.Elapsed;

        stopwatch.Restart();
        double resultWithThreads = CalculateWithThreads(totalNumbers, iterations);
        stopwatch.Stop();
        TimeSpan timeWithThreads = stopwatch.Elapsed;

        Console.WriteLine($"Result without threads: {resultWithoutThreads}");
        Console.WriteLine($"Time without threads: {timeWithoutThreads}");
        Console.WriteLine($"Result with threads: {resultWithThreads}");
        Console.WriteLine($"Time with threads: {timeWithThreads}");
    }

    static double CalculateWithoutThreads(long totalNumbers, int iterations)
    {
        double result = 0;
        for (long i = 0; i < totalNumbers; i++)
        {
            double x = i * 1.0 / totalNumbers;
            for (int j = 0; j < iterations; j++)
            {
                result += Math.Sin(x) * Math.Cos(x);
            }
        }
        return result;
    }

    static double CalculateWithThreads(long totalNumbers, int iterations)
    {
        double result = 0;
        object lockObject = new object();

        Parallel.For(0L, totalNumbers, () => 0.0, (i, state, localResult) =>
        {
            double x = i * 1.0 / totalNumbers;
            for (int j = 0; j < iterations; j++)
            {
                localResult += Math.Sin(x) * Math.Cos(x);
            }
            return localResult;
        },
        localResult =>
        {
            lock (lockObject)
            {
                result += localResult;
            }
        });

        return result;
    }
}
