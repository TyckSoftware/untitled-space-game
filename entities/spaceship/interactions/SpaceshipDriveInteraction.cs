using Godot;

/// <summary>
/// The interaction that is performed when the player
/// presses the drive interaction key.
/// </summary>
public partial class SpaceshipDriveInteraction : InteractableAreaInteraction
{
    /// <inheritdoc />
    public override void Perform()
    {
        GD.Print("SpaceshipDriveInteraction");
    }
}
