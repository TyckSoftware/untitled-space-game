using Godot;
using System;
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
    /// The entities whose types can trigger the
    /// interactable area.
    /// </summary>
    [Export]
    public string[]? EntityTypeNames { get; set; }

    /// <summary>
    /// The interactions that are registered in
    /// this area.
    /// </summary>
    [Export]
    public InteractableAreaInteraction[]? Interactions { get; set; }

    /// <summary>
    /// The interaction hints that are currently being rendered.
    /// </summary>
    private IList<CanvasItem> RenderedInteractions { get; set; }
        = new List<CanvasItem>();

    /// <summary>
    /// The input manager used to determine the
    /// input device used.
    /// </summary>
    public InputManager InputManager { get; set; } = default!;

    /// <summary>
    /// The collision shape for this interactable area.
    /// </summary>
    private CollisionShape3D? CollisionShape
        => GetChildren()
        .FirstOrDefault(child => child is CollisionShape3D) as CollisionShape3D;

    /// <summary>
    /// Whether the player is currently in the area.
    /// </summary>
    private bool IsPlayerInArea { get; set; }

    /// <inheritdoc />
    public override void _Ready()
    {
        InputManager = GetNode<InputManager>("/root/InputManager");

        if (Interactions != null)
        {
            foreach (InteractableAreaInteraction interaction in Interactions)
            {
                interaction.Initialize(this);
            }
        }
    }

    /// <inheritdoc />
    public override void _Process(double delta)
    {
        if (Interactions != null)
        {
            foreach (var interaction in Interactions)
            {
                interaction._Process(delta);
            }
        }
    }

    /// <inheritdoc />
    public override void _PhysicsProcess(double delta)
    {
        if (Interactions != null)
        {
            foreach (var interaction in Interactions)
            {
                interaction._PhysicsProcess(delta);
            }
        }
    }

    /// <summary>
    /// Method that is fired by Godot whenever
    /// a node enters the area.
    /// </summary>
    /// <param name="node">
    /// The node that entered the area.
    /// </param>
    public void OnInteractableAreaBodyEntered(Node3D node)
    {
        if (Interactions == null)
        {
            return;
        }

        // Get the name of the type of the node that entered.
        string typeName = node.GetType().ToString().ToLower().Trim();

        /*
         * Check if any provided entity type name equals the
         * type name of the node that has entered.
         */
        if (EntityTypeNames == null
            || EntityTypeNames.Length == 0
            || EntityTypeNames.Any(ecn => ecn.ToLower().Trim() == typeName))
        {
            foreach (InteractableAreaInteraction interaction in Interactions)
            {
                interaction.OnEntityEntered(node);
            }
        }
    }

    /// <summary>
    /// Method that is fired by Godot whenever
    /// a node exits the area.
    /// </summary>
    /// <param name="node">
    /// The node that exited the area.
    /// </param>
    public void OnInteractableAreaBodyExited(Node3D node)
    {
        if (Interactions == null)
        {
            return;
        }

        // Get the name of the type of the node that entered.
        string className = node.GetType().ToString().ToLower().Trim();

        /*
         * Check if any provided entity type name equals the
         * type name of the node that has entered.
         */
        if (EntityTypeNames == null
            || EntityTypeNames.Length == 0
            || EntityTypeNames.Any(ecn => ecn.ToLower().Trim() == className))
        {
            foreach (InteractableAreaInteraction interaction in Interactions)
            {
                interaction.OnEntityExited(node);
            }
        }
    }
}
