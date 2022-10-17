using Godot;

public partial class Spaceship : CharacterBody3D
{
    public const float MAX_SPEED = 50.0f;
    public const float THRUSTER_FORCE = .5f;
    private Vector3? _previousPosition;

    /// <inheritdoc />
    public override void _PhysicsProcess(double delta)
    {
        DoRotate();
        DoMove((float)delta);
        DoCameraFollow();
    }

    private void DoRotate()
    {
        float input = Input.GetAxis("turn_left", "turn_right");
        float steerValue = input * 2;
        RotateY(Mathf.DegToRad(steerValue));
    }

    private void DoMove(float delta)
    {
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Vector3 direction = (Transform.basis * new Vector3(inputDir.x, 0, inputDir.y)).Normalized();

        // Apply input force.
        if (inputDir != Vector2.Zero)
        {
            Velocity = Velocity.MoveToward(direction * MAX_SPEED, THRUSTER_FORCE);
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

    private void DoCameraFollow()
    {
        if (_previousPosition != null)
        {
            Vector3 difference = _previousPosition.Value - Position;
            difference.y = 0;

            GetViewport().GetCamera3d().GlobalTranslate(-difference);
        }

        _previousPosition = Position;
    }
}
