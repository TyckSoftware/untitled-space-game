using Godot;

/// <summary>
/// The interaction that is performed when the player
/// enters the cockpit of the spaceship.
/// </summary>
public partial class SpaceshipCockpitCollisionInteraction : InteractableAreaInteraction
{
    /// <inheritdoc />
    public override void OnEntityEntered(Node node)
    {
        base.OnEntityEntered(node);
        GD.Print("OnEntityEntered (SpaceshipCockpitCollisionInteraction)");
    }

    /// <inheritdoc />
    protected override void PhysicsProcess(double delta)
    {
        // GD.Print("PhysicsProcess (SpaceshipCockpitCollisionInteraction)");
    }

    /// <inheritdoc />
    public override void OnEntityExited(Node node)
    {
        base.OnEntityExited(node);
        GD.Print("OnEntityExited (SpaceshipCockpitCollisionInteraction)");
    }
}
