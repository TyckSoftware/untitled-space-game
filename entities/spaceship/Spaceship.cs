using Godot;

/// <summary>
/// Class containing methods and properties regarding
/// movement and rotation of a spaceship.
/// </summary>
public partial class Spaceship : RigidBody3D
{
    /// <summary>
    /// The maximum velocity that the spaceship
    /// can reach.
    /// </summary>
    public const float MAX_VELOCITY = 20f;

    /// <summary>
    /// The force that the thruster applies.
    /// </summary>
    public const float THRUSTER_FORCE = 20f;

    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
        base._IntegrateForces(state);

        ApplyVelocityClamp();

        /*
         * THIS LINE WOULD KIND OF FIX THE CONTINUOUS COLLISION DETECTION BUG:
         * https://github.com/godotengine/godot/issues/67817#issuecomment-1288172328
         */
        if (state.GetContactCount() >= 1)
        {
            //LinearVelocity = Vector3.Zero;
            //ApplyCentralImpulse(state.GetContactLocalNormal(0) * 3);
        }
    }

    /// <inheritdoc />
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        ApplyRotationTorque();
        ApplyMovementForce();
        ApplyCameraPosition();
    }

    /// <summary>
    /// Rotate the spaceship by applying a torque.
    /// </summary>
    private void ApplyRotationTorque()
    {
        float input = Input.GetAxis("turn_right", "turn_left");
        ApplyTorque(Vector3.Up * input);
    }

    /// <summary>
    /// Move the spaceship by applying a force.
    /// </summary>
    private void ApplyMovementForce()
    {
        // Apply movement force.
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Vector3 forceDirection = (Transform.basis * new Vector3(inputDir.x, 0, inputDir.y)).Normalized();

        ApplyCentralForce(forceDirection * THRUSTER_FORCE);

        // Apply braking force.
        float inputBrake = Input.GetActionStrength("brake");

        if (inputBrake != 0)
        {
            ApplyCentralForce(-LinearVelocity.Normalized() * inputBrake * THRUSTER_FORCE);
        }
    }

    /// <summary>
    /// Clamp the velocity of the spaceship.
    /// </summary>
    private void ApplyVelocityClamp()
    {
        if (LinearVelocity.Length() > MAX_VELOCITY)
        {
            LinearVelocity = LinearVelocity.Normalized() * MAX_VELOCITY;
        }
    }

    /// <summary>
    /// Move the camera to follow the spaceship
    /// by setting the position of the parent rig.
    /// </summary>
    private void ApplyCameraPosition()
    {
        var cameraRig = GetViewport().GetCamera3d().GetParent<Node3D>();
        cameraRig.GlobalPosition = Transform.origin;
    }
}
