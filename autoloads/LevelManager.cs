using Godot;
using System;

public partial class LevelManager : Node
{
	private static int _time;
    
	public static int Time 
	{ 
		get { return _time; }
	}

    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        _time += 1;
    }
}
