using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

 namespace TI {
public class MutableStack
{
    private Stack<string> _stack = new Stack<string>();
    private readonly object _lock = new object();

    public void Push(string text)
    {
        //lock (_lock)
        //{
            _stack.Push(text);
            Console.WriteLine($"Current Stack: {string.Join(", ", _stack)}");
        //}
    }

    public void Undo()
    {
        //lock (_lock)
        //{
            if (_stack.Count == 0)
            {
                Console.WriteLine("Nothing to undo.");
                return;
            }

            var lastAction = _stack.Pop();
            Console.WriteLine($"Undid action: {lastAction}");
            Console.WriteLine($"Current Stack: {string.Join(", ", _stack)}");
        //}
    }
}

// Example usage

}