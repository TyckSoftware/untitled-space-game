using System.Linq;
using Godot;

/// <inheritdoc />
/// <remarks>
///  Pertains to key events.
///  </remarks>
public partial class InteractableAreaKeyInteraction : InteractableAreaInteraction
{
    /// <summary>
    /// The key action that triggers the interaction.
    /// </summary>
    [Export]
    public string KeyAction { get; set; } = "";

    /// <summary>
    /// The text that is displayed in the UI when the
    /// interaction is available.
    /// </summary>
    [Export]
    public string ActionDisplayText { get; set; } = "";

    /// <summary>
    /// Boolean indicating whether or not the
    /// <see cref="KeyAction" is pressed.
    /// </summary>
    public bool KeyIsPressed { get; private set; }

    /// <summary>
    /// The interaction hint that is displayed when the
    /// interaction is available.
    /// </summary>
    private Control? HintControl { get; set; }

    /// <inheritdoc />
    public override void Initialize(InteractableArea interactableArea)
    {
        base.Initialize(interactableArea);

        interactableArea.InputManager.InputDeviceChanged +=
            inputEvent => RedrawInteractionHint();
    }

    /// <inheritdoc />
    protected override void Process(double delta)
    {
        if (Input.IsActionJustPressed(KeyAction))
        {
            OnKeyPressed();
        }
        else if (Input.IsActionJustReleased(KeyAction))
        {
            OnKeyReleased();
        }
    }

    /// <inheritdoc />
    protected override void OnEntitiesChanged(Node node)
    {
        if (ValidEntitiesInArea.Count > 0 && HintControl == null)
        {
            DrawInteractionHint();
        }
        else if (ValidEntitiesInArea.Count == 0 && HintControl != null)
        {
            ClearInteractionHint();
        }
    }

    #region Drawing hint

    /// <summary>
    /// Redraws the interaction hint in the UI.
    /// </summary>
    private void RedrawInteractionHint()
    {
        if (ValidEntitiesInArea.Count > 0 && HintControl != null)
        {
            ClearInteractionHint();
            DrawInteractionHint();
        }
    }

    /// <summary>
    /// Draws the interaction hint in the UI.
    /// </summary>
    private void DrawInteractionHint()
    {
        // Get the input events for the interaction.
        Godot.Collections.Array<InputEvent> inputEvents =
            InputMap.ActionGetEvents(KeyAction);

        /*
		 * Detmine the input event whose name is going to be displayed.
		 * This is done by checking what input device is used.
		 */
        InputEvent? inputEvent = null;

        switch (InteractableArea.InputManager.LatestInputEvent)
        {
            // Keyboard + Mouse
            case InputEventKey:
            case InputEventMouse:
                inputEvent = inputEvents
                    .FirstOrDefault(ie => ie is InputEventKey or InputEventMouseButton);
                break;
            // Joypad
            case InputEventJoypadButton:
            case InputEventJoypadMotion:
                inputEvent = inputEvents
                    .FirstOrDefault(ie => ie is InputEventJoypadButton or InputEventJoypadMotion);
                break;
        }

        // Get the icon for the input event.
        Image? inputIcon = inputEvent != null
                    ? InputManager.GetInputIcon(inputEvent)
                    : null;

        // Create the interaction hint label.
        HintControl = GetInteractionHintLabel(inputEvent, inputIcon);

        // Render the interaction hint label.
        InteractableArea.InteractionHintsUIContainer
            .AddChild(HintControl);
    }

    /// <summary>
    /// Gets the interaction hint label for the given interaction.
    /// </summary>
    /// <param name="inputEvent">
    /// The input event that is used to activate
    /// the interaction.
    /// </param>
    /// <param name="inputIcon">
    /// The icon that is displayed in the label.
    /// </param>
    /// <returns></returns>
    private Container GetInteractionHintLabel(InputEvent? inputEvent, Image? inputIcon)
    {
        HBoxContainer hBoxContainer = new();

        // Create a texture rect for the input icon.
        if (inputIcon != null)
        {
            ImageTexture imageTexture = ImageTexture
                .CreateFromImage(inputIcon);

            TextureRect textureRect = new()
            {
                Texture = imageTexture,
                IgnoreTextureSize = true,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                CustomMinimumSize = new Vector2i(40, 0)
            };

            hBoxContainer.AddChild(textureRect);
        }

        // Create a label with the interaction hint.
        string preMessage = string.Empty;

        if (inputEvent == null)
        {
            preMessage = "UNBOUND ";
        }
        else if (inputIcon == null)
        {
            preMessage = "MISSING ICON ";
        }

        Label label = new Label()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Text = $"{preMessage}to {ActionDisplayText}",
            LabelSettings = new()
            {
                FontSize = 24,
                OutlineSize = 3,
                OutlineColor = Colors.Black
            }
        };

        hBoxContainer.AddChild(label);

        return hBoxContainer;
    }

    /// <summary>
    /// Clears the interaction hint from the UI.
    /// </summary>
    private void ClearInteractionHint()
    {
        InteractableArea.InteractionHintsUIContainer
            .RemoveChild(HintControl);
        HintControl = null;
    }

    #endregion

    #region Input events

    /// <summary>
    /// Called when the <see cref="KeyAction" /> has
    /// been pressed.
    /// </summary>
    protected virtual void OnKeyPressed()
    {
        KeyIsPressed = true;
    }

    /// <summary>
    /// Called when the <see cref="KeyAction" /> has
    /// been released.
    /// </summary>
    protected virtual void OnKeyReleased()
    {
        KeyIsPressed = false;
    }

    #endregion
}
