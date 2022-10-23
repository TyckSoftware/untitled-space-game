using Godot;

/// <summary>
/// Class containing methods and properties regarding
/// movement and rotation of a spaceship.
/// </summary>
public partial class Spaceship : CharacterBody3D
{
    /// <summary>
    /// The maximum velocity that the spaceship
    /// can reach.
    /// </summary>
    public const float MAX_VELOCITY = 50f;

    /// <summary>
    /// The force that the thruster applies.
    /// </summary>
    public const float THRUSTER_FORCE = .5f;

    /// <summary>
    /// The direction that the spaceship is moving towards.
    /// </summary>
    private float _direction = 0f;

    /// <inheritdoc />
    public override void _PhysicsProcess(double delta)
    {
        DoRotate();
        DoMove((float)delta);
        DoCameraFollow();
    }

    /// <summary>
    /// Rotate the spaceship
    /// </summary>
    private void DoRotate()
    {
        float input = Input.GetAxis("turn_left", "turn_right");
        _direction = Mathf.Lerp(_direction, -input, .02f);
        RotateY(Mathf.DegToRad(_direction));
    }

    /// <summary>
    /// Move the spaceship.
    /// </summary>
    /// <param name="delta">
    /// The delta time.
    /// </param>
    private void DoMove(float delta)
    {
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Vector3 direction = (Transform.basis * new Vector3(inputDir.x, 0, inputDir.y)).Normalized();

        // Apply input force.
        if (inputDir != Vector2.Zero)
        {
            Velocity = Velocity.MoveToward(direction * MAX_VELOCITY, THRUSTER_FORCE);
        }

        // Apply breaking force.
        if (Input.IsKeyPressed(Key.Space))
        {
            Velocity = Velocity.MoveToward(Vector3.Zero, THRUSTER_FORCE);
        }

        // Move and detect collisions.
        KinematicCollision3D collisionInfo = MoveAndCollide(Velocity * delta);

        // Apply bounce and friction when colliding with something.
        if (collisionInfo != null)
        {
            Velocity = Velocity.Bounce(collisionInfo.GetNormal()) / 4;
        }
    }

    /// <summary>
    /// Move the camera to follow the spaceship.
    /// </summary>
    private void DoCameraFollow()
    {
        var cameraRig = GetViewport().GetCamera3d().GetParent<Node3D>();
        cameraRig.GlobalPosition = Transform.origin;
    }
}
