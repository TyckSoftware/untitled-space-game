using Godot;
using System;

public partial class Star : StaticBody3D
{	
	
	public float radius;
	public float mass;
	public float density;

	public override void _Ready()
	{
		radius = GetNode<CSGSphere3D>("Sphere").Radius;

		density = 1;
		mass = density * radius * radius * radius;
	}
	
	public override void _Process(double delta)
	{
	}
}
