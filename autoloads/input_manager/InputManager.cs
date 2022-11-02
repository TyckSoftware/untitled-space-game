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
    /// Event handler that is fired whenever the
    /// input device is changed.
    /// </summary>
    /// <param name="inputEvent">
    /// The input event that was made on the new device.
    /// </param>
    [Signal]
    public delegate void InputDeviceChangedEventHandler(InputEvent inputEvent);

    /// <summary>
    /// The collection of input event types that belong
    /// to the keyboard.
    /// </summary>
    private Type[] KeyboardInputEventTypes { get; set; } = new[]
    {
        typeof(InputEventKey),
        typeof(InputEventMouseButton),
        typeof(InputEventMouseMotion),
    };

    /// <summary>
    /// The collection of input event types that belong
    /// to the joypad.
    /// </summary>
    private Type[] JoypadInputEventTypes { get; set; } = new[]
    {
        typeof(InputEventJoypadButton),
        typeof(InputEventJoypadMotion),
    };

    public void _ready()
    {
        // Check whether all input events have icons.
        InputMap.ActionGetEvents("").ToList().ForEach(action =>
        {
            GD.Print(action);
        });
    }

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
             * Check if the input event is a joypad event
             * but the latest input event is not.
             */
            || JoypadInputEventTypes.Contains(inputEventType)
            && !JoypadInputEventTypes.Contains(latestInputEventType)
            )
        {
            // GD.Print("(InputManager) Input device changed.");
            EmitSignal(SignalName.InputDeviceChanged, inputEvent);
        }
    }

    #region GetInputIcon

    public static Image? GetInputIcon(InputEvent inputEvent)
    {
        string? inputIconPathPart = inputEvent switch
        {
            InputEventKey inputEventKey
                => GetKeyInputIconPathPart(inputEventKey),
            InputEventMouseButton inputEventMouseButton
                => GetMouseButtonInputIconPathPart(inputEventMouseButton),
            InputEventJoypadButton inputEventJoypadButton
                => GetJoypadButtonInputIconPathPart(inputEventJoypadButton),
            InputEventJoypadMotion inputEventJoypadMotion
                => GetJoypadMotionInputIconPathPart(inputEventJoypadMotion),
            _ => null,
        };

        if (inputIconPathPart == null)
        {
            return null;
        }

        try
        {
            return Image.LoadFromFile(
                $"res://autoloads/input_manager/icons/{inputIconPathPart}"
            );
        }
        catch (Exception e)
        {
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputEventKey"></param>
    /// <returns></returns>
    private static string GetKeyInputIconPathPart(
        InputEventKey inputEventKey)
    {
        string keyName = inputEventKey.PhysicalKeycode.ToString().Trim().ToLower();
        // GD.Print($"(InputManager) keyName: {keyName}");
        return $"keyboard/{keyName}.png";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputEventMouseButton"></param>
    /// <returns></returns>
    private static string GetMouseButtonInputIconPathPart(
        InputEventMouseButton inputEventMouseButton)
    {
        string keyName = inputEventMouseButton.ButtonIndex.ToString().Trim().ToLower();
        // GD.Print($"(InputManager) keyName: {keyName}");
        return $"mouse/{keyName}.png";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputEventJoypadButton"></param>
    /// <returns></returns>
    private static string GetJoypadButtonInputIconPathPart(
        InputEventJoypadButton inputEventJoypadButton)
    {
        string keyName = inputEventJoypadButton.ButtonIndex.ToString().Trim().ToLower();
        // GD.Print($"(InputManager) keyName: {keyName}");
        return $"joypad/{keyName}.png";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputEventJoypadMotion"></param>
    /// <returns></returns>
    private static string GetJoypadMotionInputIconPathPart(
        InputEventJoypadMotion inputEventJoypadMotion)
    {
        string keyName = inputEventJoypadMotion.Axis.ToString().Trim().ToLower();
        // GD.Print($"(InputManager) keyName: {keyName}");
        return $"joypad/{keyName}.png";
    }

    #endregion
}