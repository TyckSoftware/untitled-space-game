using Godot;

/// <summary>
/// Holds various input related methods and properties.
/// </summary>
public partial class InputManager : Node
{
    /// <summary>
    /// The latest input event that was made.
    /// </summary>
    /// <remarks>
    /// This can be used to check what type of
    /// input event was last made.
    /// </remarks>
    public InputEvent? LatestInputEvent { get; private set; }

    /// <summary>
    /// Event that is fired by Godot whenever
    /// any key is pressed.
    /// </summary>
    /// <param name="inputEvent"></param>
    public void _input(InputEvent inputEvent)
    {
        LatestInputEvent = inputEvent;
        // var t = inputEvent switch
        // {
        //     InputEventKey e => 1,
        //     InputEventMouseButton mouseEvent => 2,
        //     _ => 3
        // };

        // GD.Print(t);
    }
}