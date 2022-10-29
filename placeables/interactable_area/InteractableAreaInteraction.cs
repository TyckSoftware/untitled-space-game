using Godot;

/// <summary>
/// An interaction resource that can be set as an interaction
/// in the <see cref="InteractableArea"/> node.
/// </summary>
public partial class InteractableAreaInteraction : Resource
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
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsTriggered()
    {
        return Input.IsActionJustPressed(KeyAction);
    }

    /// <summary>
    /// Resources cannot be abstract, hence this method
    /// has an empty body.
    /// </summary>
    public virtual void OnInteracted()
    {
        return;
    }
}