// See https://aka.ms/new-console-template for more information
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        bool simulateTimeout = args.Length > 0 && args[0].ToLower() == "timeout";

        Console.WriteLine("Starting SSE server...");
        Console.WriteLine(simulateTimeout 
            ? "Server will simulate a timeout after 10 seconds."
            : "Server will keep the connection open for the full duration.");
        
        // Create and start the HTTP listener
        using var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5000/");
        listener.Start();

        Console.WriteLine("Server is listening on http://localhost:5000/");
        Console.WriteLine("Press Ctrl+C to stop the server.");

        while (true)
        {
            try
            {
                // Wait for a client connection
                var context = await listener.GetContextAsync();
                _ = HandleClientAsync(context, simulateTimeout);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    static async Task HandleClientAsync(HttpListenerContext context, bool simulateTimeout)
    {
        var response = context.Response;
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
        response.Headers.Add("Access-Control-Allow-Methods", "GET");

        // Handle preflight OPTIONS request
        if (context.Request.HttpMethod == "OPTIONS")
        {
            response.StatusCode = 200;
            response.Close();
            return;
        }

       
        response.ContentType = "text/event-stream";
        response.Headers.Add("Cache-Control", "no-cache");
        response.Headers.Add("Connection", "keep-alive");

        using var output = response.OutputStream;
        var writer = new StreamWriter(output);

        try
        {
            var cts = simulateTimeout 
                ? new CancellationTokenSource(TimeSpan.FromSeconds(10)) 
                : new CancellationTokenSource();

            for (int i = 1; i <= 10; i++)
            {
                if (cts.IsCancellationRequested)
                {
                    Console.WriteLine("Simulated timeout occurred.");
                    break;
                }
                var update = GenerateDeploymentUpdate(i);
                await SendSSEEventAsync(writer, "message", update);
                await Task.Delay(2000, cts.Token); // Simulate delay between updates
            }

            if (!cts.IsCancellationRequested)
            {
                await SendSSEEventAsync(writer, "complete", "Deployment completed successfully.");
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Connection timed out.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while sending events: {ex.Message}");
        }
        finally
        {
            writer.Close();
        }
    }

    static async Task SendSSEEventAsync(StreamWriter writer, string eventType, string data)
    {
        await writer.WriteLineAsync($"event: {eventType}");
        await writer.WriteLineAsync($"data: {data}");
        await writer.WriteLineAsync();
        await writer.FlushAsync();
    }

    static string GenerateDeploymentUpdate(int step)
    {
        var updates = new[]
        {
            "Initializing deployment...",
            "Validating configuration...",
            "Preparing resources...",
            "Uploading files...",
            "Configuring environment...",
            "Starting services...",
            "Running database migrations...",
            "Performing health checks...",
            "Updating DNS records...",
            "Finalizing deployment..."
        };

        return updates[step - 1];
    }
}
