using System.Collections.Generic;
using Godot;

/// <summary>
/// An interaction resource that can be set as an interaction
/// in the <see cref="InteractableArea"/> node.
/// </summary>
public interface IInteractableAreaInteraction
{
    /// <summary>
    /// The Godot groups that trigger the interaction.
    /// </summary>
    string[] Groups { get; set; }

    /// <summary>
    /// The interactable <see cref="InteractableArea"/> that
    /// the interaction is registered to.
    /// </summary>
    InteractableArea InteractableArea { get; }

    /// <summary>
    /// Entities that are currently in the area
    /// that are considered valid as they are
    /// present in the <see cref="Groups"/>.
    /// </summary>
    IList<Node> ValidEntitiesInArea { get; }

    /// <summary>
    /// Entities that are currently in the area
    /// that are considered invalid as they are
    /// not present in the <see cref="Groups"/>.
    /// </summary>
    IList<Node> InvalidEntitiesInArea { get; }

    /// <summary>
    /// Boolean indicating that an entity
    /// is present.
    /// </summary>
    bool EntityIsPresent { get; }

    /// <summary>
    /// Called when the interaction is registered to
    /// an <see cref="InteractableArea"/>.
    /// </summary>
    /// <param name="interactableArea">
    /// The interactable area that the interaction is
    /// being registered to.
    /// </param>
    void Initialize(InteractableArea interactableArea);

    /// <summary>
    /// Called every frame.
    /// </summary>
    /// <param name="delta">
    /// The elapsed time since the previous frame.
    /// </param>
    void _Process(double delta);

    /// <summary>
    /// Called every physics frame.
    /// </summary>
    /// <param name="delta">
    /// The elapsed time since the previous frame.
    /// </param>
    void _PhysicsProcess(double delta);

    /// <summary>
    /// Called when an entity entered the
    /// interactable area.
    /// </summary>
    /// <param name="node">
    /// The node that entered the interactable area.
    /// </param>
    void OnEntityEntered(Node node);

    /// <summary>
    /// Called when an entity exited the
    /// interactable area.
    /// </summary>
    /// <param name="node">
    /// The node that exited the interactable area.
    /// </param>
    void OnEntityExited(Node node);
}
