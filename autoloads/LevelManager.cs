using Godot;

/// <summary>
/// Singleton defining global level properties.
/// </summary>
public partial class LevelManager : Node
{
    /// <summary>
    /// Multiplication factor altering the rate at which time passes.
    /// </summary>
    public static float TimeIncrementFactor { get; set; } = 2.0f;

    /// <summary>
    /// Time spent in a level.
    /// </summary>
    public static float Time { get; private set; } = 0.0f;

    /// <inheritdoc />
    public override void _PhysicsProcess(double delta)
    {
        Time += TimeIncrementFactor * (float)delta;
    }
}
