using Godot;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An area that can hold a collection of
/// <see cref="InteractableAreaInteraction" /> resources
/// that will be displayed in the UI upon entering.
/// </summary>
public partial class InteractableArea : Area3D
{
    /// <summary>
    /// The UI container that the interaction hints
    /// are drawn into.
    /// </summary>
    [Export]
    public Container InteractionHintsUIContainer { get; set; }
        = default!;

    /// <summary>
    /// The interactions that are registered in
    /// this area.
    /// </summary>
    [Export]
    public InteractableAreaInteraction[] Interactions { get; set; }
        = new InteractableAreaInteraction[0];

    /// <summary>
    /// The interaction hints that are currently being rendered.
    /// </summary>
    private IList<CanvasItem> RenderedInteractions { get; set; }
        = new List<CanvasItem>();

    /// <summary>
    /// The input manager used to determine the
    /// input device used.
    /// </summary>
    private InputManager InputManager { get; set; } = default!;

    /// <summary>
    /// Whether the player is currently in the area.
    /// </summary>
    private bool IsPlayerInArea { get; set; }

    public override void _Ready()
    {
        InputManager = GetNode<InputManager>("/root/InputManager");
        InputManager.InputDeviceChanged +=
            inputEvent => OnInputDeviceChanged(inputEvent);
    }

    /// <summary>
    /// Event that is fired by Godot whenever
    /// any key is pressed.
    /// </summary>
    /// <param name="inputEvent">
    /// The input event that was made.
    /// </param>
    public void _input(InputEvent inputEvent)
    {
        if (IsPlayerInArea)
        {
            foreach (InteractableAreaInteraction interaction in Interactions)
            {
                if (Input.IsActionJustPressed(interaction.KeyAction))
                {
                    interaction.Perform();
                }
            }
        }
    }

    /// <summary>
    /// Method that is fired by the
    /// <see cref="InputManager.InputDeviceChangedEventHandler"/>
    /// whenever the input device is changed.
    /// </summary>
    /// <param name="inputEvent"></param>
    private void OnInputDeviceChanged(InputEvent inputEvent)
    {
        if (IsPlayerInArea)
        {
            ClearInteractionHints();
            DrawInteractionHints();
        }
    }

    /// <summary>
    /// Method that is fired by Godot whenever
    /// a Node3D enters the area.
    /// </summary>
    /// <param name="body">
    /// The Node3D that entered the area.
    /// </param>
    public void OnInteractableAreaBodyEntered(Node3D body)
    {
        if (body is Player)
        {
            IsPlayerInArea = true;
            DrawInteractionHints();
        }
    }

    /// <summary>
    /// Method that is fired by Godot whenever
    /// a Node3D exits the area.
    /// </summary>
    /// <param name="body">
    /// The Node3D that exited the area.
    /// </param>
    public void OnInteractableAreaBodyExited(Node3D body)
    {
        if (body is Player)
        {
            IsPlayerInArea = false;
            ClearInteractionHints();
        }
    }

    /// <summary>
    /// Draws the interaction hint for each interaction.
    /// </summary>
    private void DrawInteractionHints()
    {
        foreach (InteractableAreaInteraction interaction in Interactions)
        {
            DrawInteractionHint(interaction);
        }
    }

    /// <summary>
    /// Draws the interaction hint for the given interaction.
    /// </summary>
    /// <param name="interactableAreaInteraction">
    /// The interaction to draw the hint for.
    /// </param>
    private void DrawInteractionHint(InteractableAreaInteraction interactableAreaInteraction)
    {
        // Get the input events for the interaction.
        Godot.Collections.Array<InputEvent> inputEvents =
            InputMap.ActionGetEvents(interactableAreaInteraction.KeyAction);

        /*
		 * Detmine the input event whose name is going to be displayed.
		 * This is done by checking what input device is used.
		 */
        InputEvent? inputEvent = null;

        switch (InputManager.LatestInputEvent)
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
        Container interactionHintLabel = GetInteractionHintLabel(
            interactableAreaInteraction.ActionDisplayText,
            inputEvent,
            inputIcon
        );

        // Render the interaction hint label.
        InteractionHintsUIContainer.AddChild(interactionHintLabel);
        RenderedInteractions.Add(interactionHintLabel);
    }

    /// <summary>
    /// Clears the interaction hint for each interaction.
    /// </summary>
    private void ClearInteractionHints()
    {
        foreach (CanvasItem renderedInteraction in RenderedInteractions)
        {
            InteractionHintsUIContainer.RemoveChild(renderedInteraction);
        }

        RenderedInteractions.Clear();
    }

    /// <summary>
    /// Gets the interaction hint label for the given interaction.
    /// </summary>
    /// <param name="actionText">
    /// The text that is displayed in the label
    /// describing the action upon activation.
    /// </param>
    /// <param name="inputEvent">
    /// The input event that is used to activate
    /// the interaction.
    /// </param>
    /// <param name="inputIcon">
    /// The icon that is displayed in the label.
    /// </param>
    /// <returns></returns>
    private Container GetInteractionHintLabel(
        string actionText, InputEvent? inputEvent, Image? inputIcon)
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
            Text = $"{preMessage}to {actionText}",
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
}
