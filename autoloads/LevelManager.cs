using Godot;

/// <summary>
/// Singleton defining global level properties.
/// </summary>
public partial class LevelManager : Node
{

    /// <summary>
    /// Value by which the time increases ever frame.
    /// </summary>
    public static float TimeIncrement { get; set; } = 0.05f;

    /// <summary>
    /// Time spend in a level.
    /// </summary>
    public static float Time { get; private set; } = 0.0f;

    /// <inheritdoc />
	public override void _Process(double delta)
	{
        Time += TimeIncrement;
    }
}
