using Godot;

/// <summary>
/// Singleton defining global level properties.
/// </summary>
public partial class LevelManager : Node
{

	/// <summary>
    /// Time spend in a level.
    /// </summary>
    public static int Time { get; private set; }

    /// <inheritdoc />
	public override void _Process(double delta)
	{
        Time += 1;
    }
}
