/// <inheritdoc />
/// <remarks>
///  Pertains to collision events.
///  </remarks>
public partial class InteractableAreaCollisionInteraction : InteractableAreaInteraction
{
    /// <summary>
    /// Called when the entity entered the
    /// interactable area.
    /// </summary>
    public virtual void OnEntityEntered()
    {
        IsActive = true;
    }

    /// <summary>
    /// Called when the entity exited the
    /// interactable area.
    /// </summary>
    public virtual void OnEntityExited()
    {
        IsActive = false;
    }
}
