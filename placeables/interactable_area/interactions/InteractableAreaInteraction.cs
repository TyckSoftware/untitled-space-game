using Godot;

/// <summary>
/// An interaction resource that can be set as an interaction
/// in the <see cref="InteractableArea"/> node.
/// </summary>
public partial class InteractableAreaInteraction : Resource
{
    /// <summary>
    /// Whether the interaction is currently
    /// considered active.
    /// </summary>
    public bool IsActive { get; protected set; } = false;

    /// <summary>
    /// Called every frame that the interaction is
    /// considered active.
    /// </summary>
    /// <param name="delta">
    /// The elapsed time since the previous frame.
    /// </param>
    public virtual void Process(double delta)
    {
        return;
    }

    /// <summary>
    /// Called every physics frame that the interaction
    /// is considered active.
    /// </summary>
    /// <param name="delta">
    /// The elapsed time since the previous frame.
    /// </param>
    public virtual void PhysicsProcess(double delta)
    {
        return;
    }
}
