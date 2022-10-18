using Godot;
using System;

public partial class Star : StaticBody2D
{	
	
	public int radius = 25;
	public int mass;
	public float density;

	public override void _Ready()
	{
		density = 1;
		mass = (int)density * radius * radius * radius;
	}
	
	public override void _Process(double delta)
	{
	}
}
