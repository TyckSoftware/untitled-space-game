using Godot;

/// <summary>
/// Class containing methods and properties regarding
/// movement and rotation of the player.
/// </summary>
public partial class Player : CharacterBody3D
{
    /// <summary>
    /// The maximum velocity that the player
    /// can reach.
    /// </summary>
    public const float MAX_VELOCITY = 5.0f;

    /// <inheritdoc />
    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        // Get the input direction.
        Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Vector3 direction = (Transform.basis * new Vector3(inputDir.x, 0, inputDir.y)).Normalized();

        // Apply movement forces for accelerating and decelerating.
        if (direction != Vector3.Zero)
        {
            velocity.x = direction.x * MAX_VELOCITY;
            velocity.z = direction.z * MAX_VELOCITY;
        }
        else
        {
            velocity.x = Mathf.MoveToward(Velocity.x, 0, MAX_VELOCITY);
            velocity.z = Mathf.MoveToward(Velocity.z, 0, MAX_VELOCITY);
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
