using Godot;
using System;

public partial class Planet : StaticBody3D
{
	public float timeMultiplier = 10;
	public float radius;
	public float mass;
	public float density;
	public float meanAngularMotion;
	public float semiMajorAxis;
	public float semiMinorAxis;
	[Export]
	public Star star;
	[Export]
	public float apoapsis = 400;
	[Export]
	public float periapsis = 100;
	[Export]
	public float periapsisTime = 10;
	[Export]
	public float longitude = 45;
	[Export]
	public float periapsisArgument = 0;
	[Export]
	public float inclination = 0; 
	private double _time = 0;
	private float _eccentricity = 1;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_eccentricity = (apoapsis - periapsis) / (apoapsis + periapsis);
		// _eccentricity = Mathf.Sqrt(1 - Mathf.Pow(semiMinorAxis, 2)/Mathf.Pow(semiMajorAxis, 2));
        semiMajorAxis = 0.5f * (periapsis + apoapsis);
        semiMinorAxis = semiMajorAxis * Mathf.Sqrt(1 - Mathf.Pow(_eccentricity, 2));
		meanAngularMotion = Mathf.Sqrt(star.mass/Mathf.Pow(semiMajorAxis, 3));

		radius = GetNode<CSGSphere3D>("Sphere").Radius;

		density = 1;
		mass = density * radius * radius * radius;
	}
	
	public override void _PhysicsProcess(double delta)
	{	
		_time += timeMultiplier * delta;

		Position = OrbitalPosition((float)_time);
	}
	
	private Vector3 OrbitalPosition(float time)
	{
		float anomaly = EccentricAnomaly(time);

		Vector3 position = new Vector3(
			semiMajorAxis * (Mathf.Cos(anomaly) - _eccentricity), 
			0,
			semiMinorAxis * Mathf.Sin(anomaly)
		);

		Vector3 rotatedPosition = position.Rotated(Vector3.Up, Mathf.DegToRad(periapsisArgument));

		rotatedPosition = rotatedPosition.Rotated(Vector3.Right, Mathf.DegToRad(inclination));
		rotatedPosition = rotatedPosition.Rotated(Vector3.Up, Mathf.DegToRad(longitude));

		return rotatedPosition;
	}

	private float EccentricAnomaly(float time, int maxIterations = 20, float rTol = 1E-7f)
	{	
		// The mean anomaly
		float meanAnomaly = (time - periapsisTime) * meanAngularMotion;
		// Initial guess for E
		float E = _eccentricity > 0.8 ? Mathf.Pi : meanAnomaly;

		for (int i = 0; i < maxIterations; i++)
		{
			float numerator = _eccentricity * Mathf.Sin(E) + meanAnomaly - E;
			float denominator = _eccentricity * Mathf.Cos(E) - 1;
			
			var newE = E - numerator / denominator;

			float tolerance = Mathf.Abs((newE - E)/newE);
			
			if (tolerance <= rTol) 
			{
				return newE;
			}

			E = newE;
		}

		return E;
	}
}
