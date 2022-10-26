using Godot;

public partial class SpaceshipDriveInteraction : InteractableAreaInteraction
{
    public override void OnInteracted()
    {
        GD.Print("SpaceshipDriveInteraction");
    }
}
