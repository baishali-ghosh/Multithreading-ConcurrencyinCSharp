// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// Used when synchronous result can be returned and async behaviour is more rare
 async ValueTask<int> MethodAsync(){
    await Task.Delay(1000);
    return 13;
}


// This is better in cases where the result is often available synchronously, 
// as it can lead to better performance by reducing the overhead of creating Task objects.
ValueTask<int> GetAsyncValue(bool isReady) {
    if(isReady){
        return new ValueTask<int>(45);
    }
    else {
        return MethodAsync();
    }
}


// For comparison, you can't do
// Task<int> GetAsyncValue(bool isReady) {
//     if(isReady){
//         return new Task<int>(45);
//     }
//     else {
//         return MethodAsync();
//     }
// }

async Task ConsumingMethod() {
    // Example with synchronous result
    // A valueTask can be awaited only once
    int result1 = await GetAsyncValue(true);; // Should return 42
    Console.WriteLine($"Result when ready: {result1}");

    // Example with asynchronous result
    int result2 = await GetAsyncValue(false); // Should return 13 after delay
    Console.WriteLine($"Result when not ready: {result2}");
}

await ConsumingMethod();