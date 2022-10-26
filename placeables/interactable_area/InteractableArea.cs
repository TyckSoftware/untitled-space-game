using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class InteractableArea : Area3D
{
    /// <summary>
    /// 
    /// </summary>
    [Export]
    public Container InteractableUIContainer { get; set; }
        = default!;

    [Export]
    public InteractableAreaInteraction[] Interactions { get; set; }
        = new InteractableAreaInteraction[0];

    /// <summary>
    /// 
    /// </summary>
    private IList<Label> Labels { get; set; } = new List<Label>();

    /// <summary>
    /// The input manager used to determine the
    /// input device used.
    /// </summary>
    private InputManager InputManager { get; set; } = default!;

    public override void _Ready()
    {
        InputManager = GetNode<InputManager>("/root/InputManager");
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

    public void OnInteractableAreaBodyEntered(Node3D body)
    {
        if (body is Player)
        {
            // Add the interactions as labels to the UI container.
            Player player = (Player)body;

            foreach (InteractableAreaInteraction interaction in Interactions)
            {
                // Get the input events for the interaction.
                Godot.Collections.Array<InputEvent> inputEvents = InputMap.ActionGetEvents(interaction.KeyAction);

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
                label.Text = $"Press {keyName} to {interaction.ActionDisplayText}";
                Labels.Add(label);

                InteractableUIContainer.AddChild(label);
            }
        }
    }

    public void OnInteractableAreaBodyExited(Node3D body)
    {
        if (body is Player)
        {
            foreach (Label label in Labels)
            {
                InteractableUIContainer.RemoveChild(label);
            }

            Labels.Clear();
        }
    }
}
