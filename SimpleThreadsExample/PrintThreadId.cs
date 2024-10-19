// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


void WriteThreadId() {
    for(int i=0; i<100; i++) {
       Console.WriteLine(Thread.CurrentThread.ManagedThreadId +":"+ i);
    }
}

// WriteThreadId();
Thread thread1 = new Thread(WriteThreadId);
thread1.Start();
Thread thread2 = new Thread(WriteThreadId);
thread2.Start();
Console.ReadLine();

