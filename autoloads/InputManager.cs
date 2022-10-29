using Godot;
using System;
using System.Linq;

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
    /// 
    /// </summary>
    /// <param name="inputEvent"></param>
    [Signal]
    public delegate void InputDeviceChangedEventHandler(InputEvent inputEvent);

    private Type[] KeyboardInputEventTypes { get; set; } = new[]
    {
        typeof(InputEventKey),
        typeof(InputEventMouseButton),
        typeof(InputEventMouseMotion),
    };

    private Type[] ControllerInputEventTypes { get; set; } = new[]
    {
        typeof(InputEventJoypadButton),
        typeof(InputEventJoypadMotion),
    };

    /// <summary>
    /// Event that is fired by Godot whenever
    /// any key is pressed.
    /// </summary>
    /// <param name="inputEvent"></param>
    public void _input(InputEvent inputEvent)
    {
        Type inputEventType = inputEvent.GetType();
        Type? latestInputEventType = LatestInputEvent?.GetType();

        LatestInputEvent = inputEvent;

        if (
            latestInputEventType == null
            /*
             * Check if the input event is a keyboard event
             * but the latest input event is not.
             */
            || KeyboardInputEventTypes.Contains(inputEventType)
            && !KeyboardInputEventTypes.Contains(latestInputEventType)
            /*
             * Check if the input event is a controller event
             * but the latest input event is not.
             */
            || ControllerInputEventTypes.Contains(inputEventType)
            && !ControllerInputEventTypes.Contains(latestInputEventType)
            )
        {
            GD.Print("(InputManager) Input device changed.");
            EmitSignal(SignalName.InputDeviceChanged, inputEvent);
        }
    }
}