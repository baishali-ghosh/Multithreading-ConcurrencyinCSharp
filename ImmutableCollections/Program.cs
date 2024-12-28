using TI;
public class MutableTextEditor
{
    public static void Main()
    {
        //var editor = new MutableStack();
        var editor = new ImmutableTextEditor();

        // Start multiple threads to simulate typing and undoing
        Parallel.Invoke(
            () => editor.Push("Hello"),
            () => editor.Push(" World"),
            () => editor.Push("!")
        );

        Parallel.Invoke(
            () => editor.Undo(),
            () => editor.Undo()
        );
    }
} 