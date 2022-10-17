using Godot;

public partial class SpaceshipBackup : CharacterBody3D
{
    public const float MAX_SPEED = 10.0f;
    public const float THRUSTER_FORCE = .2f;
    private Vector3? _previousPosition;
    private int _movementOptionSelected = 0;


    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    /// <inheritdoc />
    public override void _PhysicsProcess(double delta)
    {
        DoRotate();
        DoMove();
        DoCameraFollow();
    }

    private void DoRotate()
    {
        if (_movementOptionSelected is not 3)
        {
            // Get global mouse position.
            Vector2 mousePos = GetViewport().GetMousePosition();

            Vector3 rayOrigin = GetViewport().GetCamera3d().ProjectRayOrigin(mousePos);
            Vector3 rayDirection = GetViewport().GetCamera3d().ProjectRayNormal(mousePos);

            float distance = -rayOrigin.y / rayDirection.y;
            Vector3 position = rayOrigin + rayDirection * distance;

            LookAt(position);
        }
        else
        {
            float input = Input.GetAxis("move_left", "move_right");
            float steerValue = input * 2;
            RotateY(Mathf.DegToRad(steerValue));
        }
    }

    private void DoMove()
    {
        Vector3 velocity = Velocity;

        // Get the input direction and handle the movement/deceleration.
        Vector2 inputDir = Input.GetVector("move_left", "move_right",
            _movementOptionSelected != 2 ? "move_forward" : "move_forward_mouse",
            _movementOptionSelected != 2 ? "move_back" : "move_back_mouse");

        Vector3 direction = new Vector3();

        if (_movementOptionSelected is 0)
        {
            direction = new Vector3(inputDir.x, 0, inputDir.y).Normalized();
        }
        else if (_movementOptionSelected is 1)
        {
            direction = (Transform.basis * new Vector3(inputDir.x, 0, inputDir.y)).Normalized();
        }
        else if (_movementOptionSelected is 2 or 3)
        {
            direction = (Transform.basis.z * inputDir.y).Normalized();
        }

        if (direction.x != 0)
        {
            // Apply thruster force on the X axis.
            velocity.x = Mathf.MoveToward(velocity.x, direction.x * MAX_SPEED, THRUSTER_FORCE);
        }

        if (direction.z != 0)
        {
            // Apply thruster force on the Z axis.
            velocity.z = Mathf.MoveToward(velocity.z, direction.z * MAX_SPEED, THRUSTER_FORCE);
        }

        if (Input.IsKeyPressed(Key.Space))
        {
            // Apply breaking.
            velocity.x = Mathf.MoveToward(velocity.x, 0, THRUSTER_FORCE);
            velocity.z = Mathf.MoveToward(velocity.z, 0, THRUSTER_FORCE);
        }

        Velocity = velocity;
        MoveAndSlide();
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

    public void _on_option_button_item_selected(int index)
    {
        _movementOptionSelected = index;
    }
}
