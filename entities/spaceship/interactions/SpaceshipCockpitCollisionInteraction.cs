using Godot;

/// <summary>
/// The interaction that is performed when the player
/// enters the cockpit of the spaceship.
/// </summary>
public partial class SpaceshipCockpitCollisionInteraction : InteractableAreaCollisionInteraction
{
    /// <inheritdoc />
    public override void OnEntityEntered()
    {
        base.OnEntityEntered();
        GD.Print("OnEntityEntered (SpaceshipCockpitCollisionInteraction)");
    }

    /// <inheritdoc />
    public override void PhysicsProcess(double delta)
    {
        // GD.Print("PhysicsProcess (SpaceshipCockpitCollisionInteraction)");
    }

    /// <inheritdoc />
    public override void OnEntityExited()
    {
        base.OnEntityExited();
        GD.Print("OnEntityExited (SpaceshipCockpitCollisionInteraction)");
    }
}
