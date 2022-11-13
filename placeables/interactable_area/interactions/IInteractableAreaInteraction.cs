using Godot;

/// <summary>
/// An interaction resource that can be set as an interaction
/// in the <see cref="InteractableArea"/> node.
/// </summary>
public interface IInteractableAreaInteraction
{
    /// <summary>
    /// Performs the interaction.
    /// </summary>
    void Perform();
}
