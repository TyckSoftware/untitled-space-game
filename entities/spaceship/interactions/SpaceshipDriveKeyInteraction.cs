using Godot;

/// <summary>
/// The interaction that is performed when the player
/// presses the drive interaction key.
/// </summary>
public partial class SpaceshipDriveKeyInteraction : InteractableAreaKeyInteraction
{
    /// <inheritdoc />
    protected override void OnKeyPressed()
    {
        base.OnKeyPressed();
        GD.Print("OnKeyPressed (SpaceshipDriveKeyInteraction)");
    }

    /// <inheritdoc />
    protected override void OnKeyReleased()
    {
        base.OnKeyReleased();
        GD.Print("OnKeyReleased (SpaceshipDriveKeyInteraction)");
    }
}
