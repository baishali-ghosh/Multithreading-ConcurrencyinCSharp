using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

public class ImmutableTextEditor
{
    private ImmutableStack<string> _editorStack = ImmutableStack<string>.Empty;

    public void Push(string text)
    {
        // Each operation creates a new stack instance
        _editorStack = _editorStack.Push(text);
        Console.WriteLine($"Current Stack: {string.Join(", ", _editorStack)}");
    }

    public void Undo()
    {
        if (_editorStack.IsEmpty)
        {
            Console.WriteLine("Nothing to undo.");
            return;
        }

        // Pop the last action from the immutable stack
        var (newStack, lastAction) = PopStack(_editorStack);
        _editorStack = newStack;
        Console.WriteLine($"Undid action: {lastAction}");
        Console.WriteLine($"Current Stack: {string.Join(", ", _editorStack)}");
    }

    private (ImmutableStack<string>, string) PopStack(ImmutableStack<string> stack)
    {
        // Check if the stack is empty
        if (stack.IsEmpty)
        {
            throw new InvalidOperationException("Cannot pop from an empty stack.");
        }

        // Get the last item and create a new stack without it
        var lastItem = stack.Peek();
        var newStack = stack.Pop();
        return (newStack, lastItem);
    }
}
