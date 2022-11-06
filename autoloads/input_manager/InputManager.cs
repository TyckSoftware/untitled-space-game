using Godot;
using System;
using System.Linq;

/// <summary>
/// Holds various input related methods and properties.
/// </summary>
public partial class InputManager : Node
{
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
    /// The latest input event that was made.
    /// </summary>
    /// <remarks>
    /// This can be used to check what type of
    /// input event was last made.
    /// </remarks>
    public InputEvent? LatestInputEvent { get; private set; }

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

    /// <inheritdoc />
    public void _ready()
    {
        // DebugPrintIconPaths<Key>();
        // DebugPrintIconPaths<MouseButton>();
        // DebugPrintIconPaths<JoyAxis>();
        // DebugPrintIconPaths<JoyButton>();
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

    #region Input icons

    /// <summary>
    /// Get the path of the icon for the given input event.
    /// </summary>
    /// <param name="inputEvent">
    /// The input event to get the icon path for.
    /// </param>
    /// <returns>
    /// The path of the icon for the given input event.
    /// </returns>
    public static Image? GetInputIcon(InputEvent inputEvent)
    {
        string? inputIconPathPart = inputEvent switch
        {
            InputEventKey inputEventKey
                => GetKeyInputIconPathPart(inputEventKey.PhysicalKeycode),
            InputEventMouseButton inputEventMouseButton
                => GetMouseButtonInputIconPathPart(inputEventMouseButton.ButtonIndex),
            InputEventJoypadButton inputEventJoypadButton
                => GetJoypadButtonInputIconPathPart(inputEventJoypadButton.ButtonIndex),
            InputEventJoypadMotion inputEventJoypadMotion
                => GetJoypadMotionInputIconPathPart(inputEventJoypadMotion.Axis),
            _ => null,
        };

        if (inputIconPathPart == null)
        {
            return null;
        }

        string inputIconPath = $"res://autoloads/input_manager/icons/{inputIconPathPart}";
        return Image.LoadFromFile(inputIconPath);
    }

    /// <summary>
    /// Gets the path part of the icon for the given key.
    /// </summary>
    /// <param name="keyCode">
    /// The key to get the icon path part for.
    /// </param>
    /// <returns>
    /// The path part of the icon for the given key.
    /// </returns>
    private static string GetKeyInputIconPathPart(Key keyCode)
        => $"keyboard/{keyCode.ToString().Trim().ToLower()}.png";

    /// <summary>
    /// Gets the path part of the icon for the given mouse button.
    /// </summary>
    /// <param name="mouseButtonIndex">
    /// The mouse button to get the icon path part for.
    /// </param>
    /// <returns>
    /// The path part of the icon for the given mouse button.
    /// </returns>
    private static string GetMouseButtonInputIconPathPart(
        MouseButton mouseButtonIndex)
        => $"mouse/{mouseButtonIndex.ToString().Trim().ToLower()}.png";

    /// <summary>
    /// Gets the path part of the joypad button input icon.
    /// </summary>
    /// <param name="joyButtonIndex">
    /// The joypad button index to get the path part for.
    /// </param>
    /// <returns>
    /// The path part of the joypad button input icon.
    /// </returns>
    private static string GetJoypadButtonInputIconPathPart(
        JoyButton joyButtonIndex)
        => $"joypad/{joyButtonIndex.ToString().Trim().ToLower()}.png";

    /// <summary>
    /// Gets the path part of the icon for the given joypad axis.
    /// </summary>
    /// <param name="joyAxis">
    /// The joypad axis to get the icon path part for.
    /// </param>
    /// <returns>
    /// The path part of the icon for the given joypad axis.
    /// </returns>
    private static string GetJoypadMotionInputIconPathPart(
        JoyAxis joyAxis)
        => $"joypad/{joyAxis.ToString().Trim().ToLower()}.png";

    #region Input icons (debug)

    /// <summary>
    /// Prints the icon paths for the given enum type.
    /// </summary>
    /// <typeparam name="T">
    /// The enum type to print the icon paths for.
    /// </typeparam>
    private static void DebugPrintIconPaths<T>() where T : Enum
    {
        // Check whether all input events have icons.
        var values = Enum.GetValues(typeof(T));

        foreach (var value in values)
        {
            string inputIconPathPart = GetInputIconPathPart((T)value);
            string path = $"res://autoloads/input_manager/icons/{inputIconPathPart}";

            Image image = Image.LoadFromFile(path);

            if (image == null)
            {
                GD.PrintErr($"Input icon for {value} is missing. ({path})");
            }
            else
            {
                GD.Print($"Input icon for {value} is present. ({path})");
            }
        }
    }

    /// <summary>
    /// Gets the part of the path of the icon for
    /// the given input enum.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the input enum.
    /// Must be any of the following:
    /// <see cref="Key"/>,
    /// <see cref="MouseButton"/>,
    /// <see cref="JoyAxis"/>,
    /// <see cref="JoyButton"/>.
    /// </typeparam>
    /// <param name="enumValue">
    /// The input enum to get the icon path part for.
    /// </param>
    /// <returns>
    /// The part of the path of the icon for the
    /// given input enum.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the given enum type is not one of the
    /// types listed in the type parameter.
    /// </exception>
    private static string GetInputIconPathPart<T>(T inputEnum) where T : Enum
        => inputEnum switch
        {
            Key key => GetKeyInputIconPathPart(key),
            MouseButton mouseButton => GetMouseButtonInputIconPathPart(mouseButton),
            JoyAxis joyAxis => GetJoypadMotionInputIconPathPart(joyAxis),
            JoyButton joyButton => GetJoypadButtonInputIconPathPart(joyButton),
            _ => throw new ArgumentException($"Unknown enum value: {inputEnum}"),
        };

    #endregion

    #endregion
}