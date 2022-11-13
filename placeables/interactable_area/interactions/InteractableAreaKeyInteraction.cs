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
    /// Called when the key action is pressed.
    /// </summary>
    public virtual void OnKeyPressed()
    {
        IsActive = true;
    }

    /// <summary>
    /// Called when the key action is released.
    /// </summary>
    public virtual void OnKeyReleased()
    {
        IsActive = false;
    }
}
