using Godot;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An area that can hold various interactions
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
    /// The interaction hints that are currently being displayed.
    /// </summary>
    private IList<Label> Labels { get; set; } = new List<Label>();

    /// <summary>
    /// The input manager used to determine the
    /// input device used.
    /// </summary>
    private InputManager InputManager { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    private bool IsPlayerInArea { get; set; }

    public override void _Ready()
    {
        InputManager = GetNode<InputManager>("/root/InputManager");
        InputManager.InputDeviceChanged +=
            inputEvent => OnInputDeviceChanged(inputEvent);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        foreach (InteractableAreaInteraction interaction in Interactions)
        {
            if (interaction.IsTriggered())
            {
                interaction.OnInteracted();
            }
        }
    }

    /// <summary>
    /// Event that is fired by Godot whenever
    /// any key is pressed.
    /// </summary>
    /// <param name="inputEvent"></param>
    public void _input(InputEvent inputEvent)
    {
        // GD.Print("input");
    }

    private void OnInputDeviceChanged(InputEvent inputEvent)
    {
        if (IsPlayerInArea)
        {
            ClearInteractionHints();
            DrawInteractionHints();
        }
    }

    public void OnInteractableAreaBodyEntered(Node3D body)
    {
        if (body is Player)
        {
            IsPlayerInArea = true;
            DrawInteractionHints();
        }
    }

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
            // Joypad
            case InputEventJoypadButton:
            case InputEventJoypadMotion:
                inputEvent = inputEvents.FirstOrDefault(ie => ie is InputEventJoypadButton or InputEventJoypadMotion);
                break;
            // Keyboard + Mouse
            case InputEventKey:
            case InputEventMouse:
                inputEvent = inputEvents.FirstOrDefault(ie => ie is InputEventKey or InputEventMouseButton);
                break;
        }

        string keyName = inputEvent != null ? inputEvent.AsText() : "UNBOUND";

        // Create a label with the interaction hint.
        Label label = new Label();
        label.Text = $"Press {keyName} to {interactableAreaInteraction.ActionDisplayText}";
        Labels.Add(label);

        InteractionHintsUIContainer.AddChild(label);
    }

    /// <summary>
    /// Clears the interaction hint for each interaction.
    /// </summary>
    private void ClearInteractionHints()
    {
        foreach (Label label in Labels)
        {
            InteractionHintsUIContainer.RemoveChild(label);
        }

        Labels.Clear();
    }
}
